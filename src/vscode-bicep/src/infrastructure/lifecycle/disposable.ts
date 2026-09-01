// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Disposable as VscodeDisposable } from "vscode";

export abstract class Disposable {
  private readonly disposables: VscodeDisposable[] = [];

  private disposed = false;

  public dispose(): void {
    if (this.disposed) {
      return;
    }

    this.disposed = true;

    while (this.disposables.length) {
      this.disposables.pop()?.dispose();
    }
  }

  protected get isDisposed(): boolean {
    return this.disposed;
  }

  public register<T extends VscodeDisposable>(disposable: T): T {
    if (this.disposed) {
      disposable.dispose();
    } else {
      this.disposables.push(disposable);
    }

    return disposable;
  }

  public registerMultiple<T extends VscodeDisposable[]>(...disposables: T): T {
    return disposables.map((disposable) => this.register(disposable)) as T;
  }
}
