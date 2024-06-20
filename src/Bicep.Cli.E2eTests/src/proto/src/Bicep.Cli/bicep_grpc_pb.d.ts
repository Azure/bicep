// package: bicep
// file: src/Bicep.Cli/bicep.proto

/* tslint:disable */
/* eslint-disable */

import * as grpc from "@grpc/grpc-js";
import * as src_Bicep_Cli_bicep_pb from "../../src/Bicep.Cli/bicep_pb";

interface IRpcService extends grpc.ServiceDefinition<grpc.UntypedServiceImplementation> {
    version: IRpcService_IVersion;
    compile: IRpcService_ICompile;
    compileParams: IRpcService_ICompileParams;
    getMetadata: IRpcService_IGetMetadata;
    getDeploymentGraph: IRpcService_IGetDeploymentGraph;
    getFileReferences: IRpcService_IGetFileReferences;
}

interface IRpcService_IVersion extends grpc.MethodDefinition<src_Bicep_Cli_bicep_pb.VersionRequest, src_Bicep_Cli_bicep_pb.VersionResponse> {
    path: "/bicep.Rpc/Version";
    requestStream: false;
    responseStream: false;
    requestSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.VersionRequest>;
    requestDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.VersionRequest>;
    responseSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.VersionResponse>;
    responseDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.VersionResponse>;
}
interface IRpcService_ICompile extends grpc.MethodDefinition<src_Bicep_Cli_bicep_pb.CompileRequest, src_Bicep_Cli_bicep_pb.CompileResponse> {
    path: "/bicep.Rpc/Compile";
    requestStream: false;
    responseStream: false;
    requestSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.CompileRequest>;
    requestDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.CompileRequest>;
    responseSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.CompileResponse>;
    responseDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.CompileResponse>;
}
interface IRpcService_ICompileParams extends grpc.MethodDefinition<src_Bicep_Cli_bicep_pb.CompileParamsRequest, src_Bicep_Cli_bicep_pb.CompileParamsResponse> {
    path: "/bicep.Rpc/CompileParams";
    requestStream: false;
    responseStream: false;
    requestSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.CompileParamsRequest>;
    requestDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.CompileParamsRequest>;
    responseSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.CompileParamsResponse>;
    responseDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.CompileParamsResponse>;
}
interface IRpcService_IGetMetadata extends grpc.MethodDefinition<src_Bicep_Cli_bicep_pb.GetMetadataRequest, src_Bicep_Cli_bicep_pb.GetMetadataResponse> {
    path: "/bicep.Rpc/GetMetadata";
    requestStream: false;
    responseStream: false;
    requestSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.GetMetadataRequest>;
    requestDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.GetMetadataRequest>;
    responseSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.GetMetadataResponse>;
    responseDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.GetMetadataResponse>;
}
interface IRpcService_IGetDeploymentGraph extends grpc.MethodDefinition<src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse> {
    path: "/bicep.Rpc/GetDeploymentGraph";
    requestStream: false;
    responseStream: false;
    requestSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest>;
    requestDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest>;
    responseSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse>;
    responseDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse>;
}
interface IRpcService_IGetFileReferences extends grpc.MethodDefinition<src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, src_Bicep_Cli_bicep_pb.GetFileReferencesResponse> {
    path: "/bicep.Rpc/GetFileReferences";
    requestStream: false;
    responseStream: false;
    requestSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.GetFileReferencesRequest>;
    requestDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.GetFileReferencesRequest>;
    responseSerialize: grpc.serialize<src_Bicep_Cli_bicep_pb.GetFileReferencesResponse>;
    responseDeserialize: grpc.deserialize<src_Bicep_Cli_bicep_pb.GetFileReferencesResponse>;
}

export const RpcService: IRpcService;

export interface IRpcServer extends grpc.UntypedServiceImplementation {
    version: grpc.handleUnaryCall<src_Bicep_Cli_bicep_pb.VersionRequest, src_Bicep_Cli_bicep_pb.VersionResponse>;
    compile: grpc.handleUnaryCall<src_Bicep_Cli_bicep_pb.CompileRequest, src_Bicep_Cli_bicep_pb.CompileResponse>;
    compileParams: grpc.handleUnaryCall<src_Bicep_Cli_bicep_pb.CompileParamsRequest, src_Bicep_Cli_bicep_pb.CompileParamsResponse>;
    getMetadata: grpc.handleUnaryCall<src_Bicep_Cli_bicep_pb.GetMetadataRequest, src_Bicep_Cli_bicep_pb.GetMetadataResponse>;
    getDeploymentGraph: grpc.handleUnaryCall<src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse>;
    getFileReferences: grpc.handleUnaryCall<src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, src_Bicep_Cli_bicep_pb.GetFileReferencesResponse>;
}

export interface IRpcClient {
    version(request: src_Bicep_Cli_bicep_pb.VersionRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.VersionResponse) => void): grpc.ClientUnaryCall;
    version(request: src_Bicep_Cli_bicep_pb.VersionRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.VersionResponse) => void): grpc.ClientUnaryCall;
    version(request: src_Bicep_Cli_bicep_pb.VersionRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.VersionResponse) => void): grpc.ClientUnaryCall;
    compile(request: src_Bicep_Cli_bicep_pb.CompileRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileResponse) => void): grpc.ClientUnaryCall;
    compile(request: src_Bicep_Cli_bicep_pb.CompileRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileResponse) => void): grpc.ClientUnaryCall;
    compile(request: src_Bicep_Cli_bicep_pb.CompileRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileResponse) => void): grpc.ClientUnaryCall;
    compileParams(request: src_Bicep_Cli_bicep_pb.CompileParamsRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileParamsResponse) => void): grpc.ClientUnaryCall;
    compileParams(request: src_Bicep_Cli_bicep_pb.CompileParamsRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileParamsResponse) => void): grpc.ClientUnaryCall;
    compileParams(request: src_Bicep_Cli_bicep_pb.CompileParamsRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileParamsResponse) => void): grpc.ClientUnaryCall;
    getMetadata(request: src_Bicep_Cli_bicep_pb.GetMetadataRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetMetadataResponse) => void): grpc.ClientUnaryCall;
    getMetadata(request: src_Bicep_Cli_bicep_pb.GetMetadataRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetMetadataResponse) => void): grpc.ClientUnaryCall;
    getMetadata(request: src_Bicep_Cli_bicep_pb.GetMetadataRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetMetadataResponse) => void): grpc.ClientUnaryCall;
    getDeploymentGraph(request: src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse) => void): grpc.ClientUnaryCall;
    getDeploymentGraph(request: src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse) => void): grpc.ClientUnaryCall;
    getDeploymentGraph(request: src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse) => void): grpc.ClientUnaryCall;
    getFileReferences(request: src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetFileReferencesResponse) => void): grpc.ClientUnaryCall;
    getFileReferences(request: src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetFileReferencesResponse) => void): grpc.ClientUnaryCall;
    getFileReferences(request: src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetFileReferencesResponse) => void): grpc.ClientUnaryCall;
}

export class RpcClient extends grpc.Client implements IRpcClient {
    constructor(address: string, credentials: grpc.ChannelCredentials, options?: Partial<grpc.ClientOptions>);
    public version(request: src_Bicep_Cli_bicep_pb.VersionRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.VersionResponse) => void): grpc.ClientUnaryCall;
    public version(request: src_Bicep_Cli_bicep_pb.VersionRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.VersionResponse) => void): grpc.ClientUnaryCall;
    public version(request: src_Bicep_Cli_bicep_pb.VersionRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.VersionResponse) => void): grpc.ClientUnaryCall;
    public compile(request: src_Bicep_Cli_bicep_pb.CompileRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileResponse) => void): grpc.ClientUnaryCall;
    public compile(request: src_Bicep_Cli_bicep_pb.CompileRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileResponse) => void): grpc.ClientUnaryCall;
    public compile(request: src_Bicep_Cli_bicep_pb.CompileRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileResponse) => void): grpc.ClientUnaryCall;
    public compileParams(request: src_Bicep_Cli_bicep_pb.CompileParamsRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileParamsResponse) => void): grpc.ClientUnaryCall;
    public compileParams(request: src_Bicep_Cli_bicep_pb.CompileParamsRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileParamsResponse) => void): grpc.ClientUnaryCall;
    public compileParams(request: src_Bicep_Cli_bicep_pb.CompileParamsRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.CompileParamsResponse) => void): grpc.ClientUnaryCall;
    public getMetadata(request: src_Bicep_Cli_bicep_pb.GetMetadataRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetMetadataResponse) => void): grpc.ClientUnaryCall;
    public getMetadata(request: src_Bicep_Cli_bicep_pb.GetMetadataRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetMetadataResponse) => void): grpc.ClientUnaryCall;
    public getMetadata(request: src_Bicep_Cli_bicep_pb.GetMetadataRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetMetadataResponse) => void): grpc.ClientUnaryCall;
    public getDeploymentGraph(request: src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse) => void): grpc.ClientUnaryCall;
    public getDeploymentGraph(request: src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse) => void): grpc.ClientUnaryCall;
    public getDeploymentGraph(request: src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse) => void): grpc.ClientUnaryCall;
    public getFileReferences(request: src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetFileReferencesResponse) => void): grpc.ClientUnaryCall;
    public getFileReferences(request: src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, metadata: grpc.Metadata, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetFileReferencesResponse) => void): grpc.ClientUnaryCall;
    public getFileReferences(request: src_Bicep_Cli_bicep_pb.GetFileReferencesRequest, metadata: grpc.Metadata, options: Partial<grpc.CallOptions>, callback: (error: grpc.ServiceError | null, response: src_Bicep_Cli_bicep_pb.GetFileReferencesResponse) => void): grpc.ClientUnaryCall;
}
