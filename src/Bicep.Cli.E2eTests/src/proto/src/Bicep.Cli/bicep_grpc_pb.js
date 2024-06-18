// GENERATED CODE -- DO NOT EDIT!

'use strict';
var grpc = require('@grpc/grpc-js');
var src_Bicep_Cli_bicep_pb = require('../../src/Bicep.Cli/bicep_pb.js');

function serialize_bicep_CompileParamsRequest(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.CompileParamsRequest)) {
    throw new Error('Expected argument of type bicep.CompileParamsRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_CompileParamsRequest(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.CompileParamsRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_CompileParamsResponse(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.CompileParamsResponse)) {
    throw new Error('Expected argument of type bicep.CompileParamsResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_CompileParamsResponse(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.CompileParamsResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_CompileRequest(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.CompileRequest)) {
    throw new Error('Expected argument of type bicep.CompileRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_CompileRequest(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.CompileRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_CompileResponse(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.CompileResponse)) {
    throw new Error('Expected argument of type bicep.CompileResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_CompileResponse(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.CompileResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_GetDeploymentGraphRequest(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest)) {
    throw new Error('Expected argument of type bicep.GetDeploymentGraphRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_GetDeploymentGraphRequest(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_GetDeploymentGraphResponse(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse)) {
    throw new Error('Expected argument of type bicep.GetDeploymentGraphResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_GetDeploymentGraphResponse(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_GetFileReferencesRequest(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.GetFileReferencesRequest)) {
    throw new Error('Expected argument of type bicep.GetFileReferencesRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_GetFileReferencesRequest(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.GetFileReferencesRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_GetFileReferencesResponse(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.GetFileReferencesResponse)) {
    throw new Error('Expected argument of type bicep.GetFileReferencesResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_GetFileReferencesResponse(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.GetFileReferencesResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_GetMetadataRequest(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.GetMetadataRequest)) {
    throw new Error('Expected argument of type bicep.GetMetadataRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_GetMetadataRequest(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.GetMetadataRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_GetMetadataResponse(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.GetMetadataResponse)) {
    throw new Error('Expected argument of type bicep.GetMetadataResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_GetMetadataResponse(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.GetMetadataResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_VersionRequest(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.VersionRequest)) {
    throw new Error('Expected argument of type bicep.VersionRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_VersionRequest(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.VersionRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_bicep_VersionResponse(arg) {
  if (!(arg instanceof src_Bicep_Cli_bicep_pb.VersionResponse)) {
    throw new Error('Expected argument of type bicep.VersionResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_bicep_VersionResponse(buffer_arg) {
  return src_Bicep_Cli_bicep_pb.VersionResponse.deserializeBinary(new Uint8Array(buffer_arg));
}


var RpcService = exports.RpcService = {
  version: {
    path: '/bicep.Rpc/Version',
    requestStream: false,
    responseStream: false,
    requestType: src_Bicep_Cli_bicep_pb.VersionRequest,
    responseType: src_Bicep_Cli_bicep_pb.VersionResponse,
    requestSerialize: serialize_bicep_VersionRequest,
    requestDeserialize: deserialize_bicep_VersionRequest,
    responseSerialize: serialize_bicep_VersionResponse,
    responseDeserialize: deserialize_bicep_VersionResponse,
  },
  compile: {
    path: '/bicep.Rpc/Compile',
    requestStream: false,
    responseStream: false,
    requestType: src_Bicep_Cli_bicep_pb.CompileRequest,
    responseType: src_Bicep_Cli_bicep_pb.CompileResponse,
    requestSerialize: serialize_bicep_CompileRequest,
    requestDeserialize: deserialize_bicep_CompileRequest,
    responseSerialize: serialize_bicep_CompileResponse,
    responseDeserialize: deserialize_bicep_CompileResponse,
  },
  compileParams: {
    path: '/bicep.Rpc/CompileParams',
    requestStream: false,
    responseStream: false,
    requestType: src_Bicep_Cli_bicep_pb.CompileParamsRequest,
    responseType: src_Bicep_Cli_bicep_pb.CompileParamsResponse,
    requestSerialize: serialize_bicep_CompileParamsRequest,
    requestDeserialize: deserialize_bicep_CompileParamsRequest,
    responseSerialize: serialize_bicep_CompileParamsResponse,
    responseDeserialize: deserialize_bicep_CompileParamsResponse,
  },
  getMetadata: {
    path: '/bicep.Rpc/GetMetadata',
    requestStream: false,
    responseStream: false,
    requestType: src_Bicep_Cli_bicep_pb.GetMetadataRequest,
    responseType: src_Bicep_Cli_bicep_pb.GetMetadataResponse,
    requestSerialize: serialize_bicep_GetMetadataRequest,
    requestDeserialize: deserialize_bicep_GetMetadataRequest,
    responseSerialize: serialize_bicep_GetMetadataResponse,
    responseDeserialize: deserialize_bicep_GetMetadataResponse,
  },
  getDeploymentGraph: {
    path: '/bicep.Rpc/GetDeploymentGraph',
    requestStream: false,
    responseStream: false,
    requestType: src_Bicep_Cli_bicep_pb.GetDeploymentGraphRequest,
    responseType: src_Bicep_Cli_bicep_pb.GetDeploymentGraphResponse,
    requestSerialize: serialize_bicep_GetDeploymentGraphRequest,
    requestDeserialize: deserialize_bicep_GetDeploymentGraphRequest,
    responseSerialize: serialize_bicep_GetDeploymentGraphResponse,
    responseDeserialize: deserialize_bicep_GetDeploymentGraphResponse,
  },
  getFileReferences: {
    path: '/bicep.Rpc/GetFileReferences',
    requestStream: false,
    responseStream: false,
    requestType: src_Bicep_Cli_bicep_pb.GetFileReferencesRequest,
    responseType: src_Bicep_Cli_bicep_pb.GetFileReferencesResponse,
    requestSerialize: serialize_bicep_GetFileReferencesRequest,
    requestDeserialize: deserialize_bicep_GetFileReferencesRequest,
    responseSerialize: serialize_bicep_GetFileReferencesResponse,
    responseDeserialize: deserialize_bicep_GetFileReferencesResponse,
  },
};

exports.RpcClient = grpc.makeGenericClientConstructor(RpcService);
