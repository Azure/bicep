// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AccessToken, TokenCredential } from "@azure/core-auth";

import {
  authentication,
  AuthenticationGetSessionOptions,
  AuthenticationSession,
  AuthenticationSessionAccountInformation,
} from "vscode";
import {
  AzureEnvironment,
  getAzureAuthenticationProviderId,
  getAzureResourceManagerClientOptions,
  getAzureScopes,
  getConfiguredAzureEnvironment,
} from "./azure-environment";

export interface AzureSubscription {
  readonly account: AuthenticationSessionAccountInformation;
  readonly credential: TokenCredential;
  readonly environment: AzureEnvironment;
  readonly isCustomCloud: boolean;
  readonly name: string;
  readonly subscriptionId: string;
  readonly tenantId: string;
}

export interface AzureTenant {
  readonly account: AuthenticationSessionAccountInformation;
  readonly tenantId: string;
}

export class AzureAccountManager {
  public async getSubscriptions(): Promise<AzureSubscription[]> {
    const environment = getConfiguredAzureEnvironment();
    const accounts = await authentication.getAccounts(getAzureAuthenticationProviderId(environment));
    const subscriptions: AzureSubscription[] = [];

    for (const account of accounts) {
      const tenantIds = new Set<string | undefined>([undefined]);
      for (const tenant of await this.getTenants(account)) {
        tenantIds.add(tenant.tenantId);
      }

      for (const tenantId of tenantIds) {
        const session = await getSession(environment, tenantId, { account, silent: true });
        if (!session) {
          continue;
        }

        const credential = createCredential(environment, account, tenantId);
        const { SubscriptionClient } = await import("@azure/arm-resources-subscriptions");
        const client = new SubscriptionClient(credential, getAzureResourceManagerClientOptions(environment));
        for await (const subscription of client.subscriptions.list()) {
          if (!subscription.displayName || !subscription.subscriptionId) {
            continue;
          }

          const subscriptionTenantId = tenantId ?? subscription.tenantId;
          if (!subscriptionTenantId) {
            continue;
          }

          subscriptions.push({
            account,
            credential,
            environment,
            isCustomCloud: environment.isCustomCloud,
            name: subscription.displayName,
            subscriptionId: subscription.subscriptionId,
            tenantId: subscriptionTenantId,
          });
        }
      }
    }

    const uniqueSubscriptions = new Map<string, AzureSubscription>();
    for (const subscription of subscriptions) {
      const key = `${subscription.account.id}/${subscription.tenantId}/${subscription.subscriptionId}`;
      uniqueSubscriptions.set(key, subscription);
    }

    return [...uniqueSubscriptions.values()].sort((left, right) => left.name.localeCompare(right.name));
  }

  public async getTenants(account?: AuthenticationSessionAccountInformation): Promise<AzureTenant[]> {
    const environment = getConfiguredAzureEnvironment();
    const accounts = account
      ? [account]
      : await authentication.getAccounts(getAzureAuthenticationProviderId(environment));
    const tenants: AzureTenant[] = [];

    for (const currentAccount of accounts) {
      const session = await getSession(environment, undefined, { account: currentAccount, silent: true });
      if (!session) {
        continue;
      }

      const credential = createCredential(environment, currentAccount);
      const { SubscriptionClient } = await import("@azure/arm-resources-subscriptions");
      const client = new SubscriptionClient(credential, getAzureResourceManagerClientOptions(environment));
      for await (const tenant of client.tenants.list()) {
        if (tenant.tenantId) {
          tenants.push({ account: currentAccount, tenantId: tenant.tenantId });
        }
      }
    }

    return tenants;
  }

  public async isSignedIn(tenantId?: string, account?: AuthenticationSessionAccountInformation): Promise<boolean> {
    const environment = getConfiguredAzureEnvironment();
    if (account) {
      return !!(await getSession(environment, tenantId, { account, silent: true }));
    }

    const accounts = await authentication.getAccounts(getAzureAuthenticationProviderId(environment));
    for (const currentAccount of accounts) {
      if (await getSession(environment, tenantId, { account: currentAccount, silent: true })) {
        return true;
      }
    }

    return false;
  }

  public async signIn(): Promise<boolean> {
    const environment = getConfiguredAzureEnvironment();
    return !!(await getSession(environment, undefined, {
      clearSessionPreference: true,
      createIfNone: true,
    }));
  }
}

export async function getAzureAccessToken(subscription: AzureSubscription): Promise<AccessToken> {
  const token = await subscription.credential.getToken(getAzureScopes(subscription.environment, subscription.tenantId));
  if (!token) {
    throw new Error(`Unable to get an access token for subscription '${subscription.subscriptionId}'.`);
  }

  return token;
}

function createCredential(
  environment: AzureEnvironment,
  account: AuthenticationSessionAccountInformation,
  tenantId?: string,
): TokenCredential {
  return {
    getToken: async (scopes): Promise<AccessToken | null> => {
      const session = await getSession(environment, tenantId, { account, silent: true }, scopes);
      if (!session) {
        return null;
      }

      return {
        token: session.accessToken,
        expiresOnTimestamp: Date.now() + 5 * 60 * 1000,
      };
    },
  };
}

async function getSession(
  environment: AzureEnvironment,
  tenantId: string | undefined,
  options: AuthenticationGetSessionOptions,
  scopes?: string | string[],
): Promise<AuthenticationSession | undefined> {
  return await authentication.getSession(
    getAzureAuthenticationProviderId(environment),
    getAzureScopes(environment, tenantId, scopes),
    options,
  );
}
