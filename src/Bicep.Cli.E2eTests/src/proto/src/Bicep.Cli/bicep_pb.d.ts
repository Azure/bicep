// package: bicep
// file: src/Bicep.Cli/bicep.proto

/* tslint:disable */
/* eslint-disable */

import * as jspb from "google-protobuf";

export class Position extends jspb.Message { 
    getLine(): number;
    setLine(value: number): Position;
    getChar(): number;
    setChar(value: number): Position;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): Position.AsObject;
    static toObject(includeInstance: boolean, msg: Position): Position.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: Position, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): Position;
    static deserializeBinaryFromReader(message: Position, reader: jspb.BinaryReader): Position;
}

export namespace Position {
    export type AsObject = {
        line: number,
        pb_char: number,
    }
}

export class Range extends jspb.Message { 

    hasStart(): boolean;
    clearStart(): void;
    getStart(): Position | undefined;
    setStart(value?: Position): Range;

    hasEnd(): boolean;
    clearEnd(): void;
    getEnd(): Position | undefined;
    setEnd(value?: Position): Range;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): Range.AsObject;
    static toObject(includeInstance: boolean, msg: Range): Range.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: Range, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): Range;
    static deserializeBinaryFromReader(message: Range, reader: jspb.BinaryReader): Range;
}

export namespace Range {
    export type AsObject = {
        start?: Position.AsObject,
        end?: Position.AsObject,
    }
}

export class VersionRequest extends jspb.Message { 

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): VersionRequest.AsObject;
    static toObject(includeInstance: boolean, msg: VersionRequest): VersionRequest.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: VersionRequest, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): VersionRequest;
    static deserializeBinaryFromReader(message: VersionRequest, reader: jspb.BinaryReader): VersionRequest;
}

export namespace VersionRequest {
    export type AsObject = {
    }
}

export class VersionResponse extends jspb.Message { 
    getVersion(): string;
    setVersion(value: string): VersionResponse;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): VersionResponse.AsObject;
    static toObject(includeInstance: boolean, msg: VersionResponse): VersionResponse.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: VersionResponse, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): VersionResponse;
    static deserializeBinaryFromReader(message: VersionResponse, reader: jspb.BinaryReader): VersionResponse;
}

export namespace VersionResponse {
    export type AsObject = {
        version: string,
    }
}

export class CompileRequest extends jspb.Message { 
    getPath(): string;
    setPath(value: string): CompileRequest;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): CompileRequest.AsObject;
    static toObject(includeInstance: boolean, msg: CompileRequest): CompileRequest.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: CompileRequest, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): CompileRequest;
    static deserializeBinaryFromReader(message: CompileRequest, reader: jspb.BinaryReader): CompileRequest;
}

export namespace CompileRequest {
    export type AsObject = {
        path: string,
    }
}

export class CompileResponse extends jspb.Message { 
    getSuccess(): boolean;
    setSuccess(value: boolean): CompileResponse;
    clearDiagnosticsList(): void;
    getDiagnosticsList(): Array<Diagnostic>;
    setDiagnosticsList(value: Array<Diagnostic>): CompileResponse;
    addDiagnostics(value?: Diagnostic, index?: number): Diagnostic;

    hasContents(): boolean;
    clearContents(): void;
    getContents(): string | undefined;
    setContents(value: string): CompileResponse;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): CompileResponse.AsObject;
    static toObject(includeInstance: boolean, msg: CompileResponse): CompileResponse.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: CompileResponse, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): CompileResponse;
    static deserializeBinaryFromReader(message: CompileResponse, reader: jspb.BinaryReader): CompileResponse;
}

export namespace CompileResponse {
    export type AsObject = {
        success: boolean,
        diagnosticsList: Array<Diagnostic.AsObject>,
        contents?: string,
    }
}

export class CompileParamsRequest extends jspb.Message { 
    getPath(): string;
    setPath(value: string): CompileParamsRequest;

    getParameteroverridesMap(): jspb.Map<string, string>;
    clearParameteroverridesMap(): void;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): CompileParamsRequest.AsObject;
    static toObject(includeInstance: boolean, msg: CompileParamsRequest): CompileParamsRequest.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: CompileParamsRequest, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): CompileParamsRequest;
    static deserializeBinaryFromReader(message: CompileParamsRequest, reader: jspb.BinaryReader): CompileParamsRequest;
}

export namespace CompileParamsRequest {
    export type AsObject = {
        path: string,

        parameteroverridesMap: Array<[string, string]>,
    }
}

export class CompileParamsResponse extends jspb.Message { 
    getSuccess(): boolean;
    setSuccess(value: boolean): CompileParamsResponse;
    clearDiagnosticsList(): void;
    getDiagnosticsList(): Array<Diagnostic>;
    setDiagnosticsList(value: Array<Diagnostic>): CompileParamsResponse;
    addDiagnostics(value?: Diagnostic, index?: number): Diagnostic;

    hasParameters(): boolean;
    clearParameters(): void;
    getParameters(): string | undefined;
    setParameters(value: string): CompileParamsResponse;

    hasTemplate(): boolean;
    clearTemplate(): void;
    getTemplate(): string | undefined;
    setTemplate(value: string): CompileParamsResponse;

    hasTemplatespecid(): boolean;
    clearTemplatespecid(): void;
    getTemplatespecid(): string | undefined;
    setTemplatespecid(value: string): CompileParamsResponse;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): CompileParamsResponse.AsObject;
    static toObject(includeInstance: boolean, msg: CompileParamsResponse): CompileParamsResponse.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: CompileParamsResponse, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): CompileParamsResponse;
    static deserializeBinaryFromReader(message: CompileParamsResponse, reader: jspb.BinaryReader): CompileParamsResponse;
}

export namespace CompileParamsResponse {
    export type AsObject = {
        success: boolean,
        diagnosticsList: Array<Diagnostic.AsObject>,
        parameters?: string,
        template?: string,
        templatespecid?: string,
    }
}

export class Diagnostic extends jspb.Message { 
    getSource(): string;
    setSource(value: string): Diagnostic;

    hasRange(): boolean;
    clearRange(): void;
    getRange(): Range | undefined;
    setRange(value?: Range): Diagnostic;
    getLevel(): string;
    setLevel(value: string): Diagnostic;
    getCode(): string;
    setCode(value: string): Diagnostic;
    getMessage(): string;
    setMessage(value: string): Diagnostic;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): Diagnostic.AsObject;
    static toObject(includeInstance: boolean, msg: Diagnostic): Diagnostic.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: Diagnostic, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): Diagnostic;
    static deserializeBinaryFromReader(message: Diagnostic, reader: jspb.BinaryReader): Diagnostic;
}

export namespace Diagnostic {
    export type AsObject = {
        source: string,
        range?: Range.AsObject,
        level: string,
        code: string,
        message: string,
    }
}

export class GetFileReferencesRequest extends jspb.Message { 
    getPath(): string;
    setPath(value: string): GetFileReferencesRequest;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): GetFileReferencesRequest.AsObject;
    static toObject(includeInstance: boolean, msg: GetFileReferencesRequest): GetFileReferencesRequest.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: GetFileReferencesRequest, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): GetFileReferencesRequest;
    static deserializeBinaryFromReader(message: GetFileReferencesRequest, reader: jspb.BinaryReader): GetFileReferencesRequest;
}

export namespace GetFileReferencesRequest {
    export type AsObject = {
        path: string,
    }
}

export class GetFileReferencesResponse extends jspb.Message { 
    clearFilepathsList(): void;
    getFilepathsList(): Array<string>;
    setFilepathsList(value: Array<string>): GetFileReferencesResponse;
    addFilepaths(value: string, index?: number): string;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): GetFileReferencesResponse.AsObject;
    static toObject(includeInstance: boolean, msg: GetFileReferencesResponse): GetFileReferencesResponse.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: GetFileReferencesResponse, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): GetFileReferencesResponse;
    static deserializeBinaryFromReader(message: GetFileReferencesResponse, reader: jspb.BinaryReader): GetFileReferencesResponse;
}

export namespace GetFileReferencesResponse {
    export type AsObject = {
        filepathsList: Array<string>,
    }
}

export class GetMetadataRequest extends jspb.Message { 
    getPath(): string;
    setPath(value: string): GetMetadataRequest;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): GetMetadataRequest.AsObject;
    static toObject(includeInstance: boolean, msg: GetMetadataRequest): GetMetadataRequest.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: GetMetadataRequest, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): GetMetadataRequest;
    static deserializeBinaryFromReader(message: GetMetadataRequest, reader: jspb.BinaryReader): GetMetadataRequest;
}

export namespace GetMetadataRequest {
    export type AsObject = {
        path: string,
    }
}

export class GetMetadataResponse extends jspb.Message { 
    clearMetadataList(): void;
    getMetadataList(): Array<GetMetadataResponse.MetadataDefinition>;
    setMetadataList(value: Array<GetMetadataResponse.MetadataDefinition>): GetMetadataResponse;
    addMetadata(value?: GetMetadataResponse.MetadataDefinition, index?: number): GetMetadataResponse.MetadataDefinition;
    clearParametersList(): void;
    getParametersList(): Array<GetMetadataResponse.SymbolDefinition>;
    setParametersList(value: Array<GetMetadataResponse.SymbolDefinition>): GetMetadataResponse;
    addParameters(value?: GetMetadataResponse.SymbolDefinition, index?: number): GetMetadataResponse.SymbolDefinition;
    clearOutputsList(): void;
    getOutputsList(): Array<GetMetadataResponse.SymbolDefinition>;
    setOutputsList(value: Array<GetMetadataResponse.SymbolDefinition>): GetMetadataResponse;
    addOutputs(value?: GetMetadataResponse.SymbolDefinition, index?: number): GetMetadataResponse.SymbolDefinition;
    clearExportsList(): void;
    getExportsList(): Array<GetMetadataResponse.ExportDefinition>;
    setExportsList(value: Array<GetMetadataResponse.ExportDefinition>): GetMetadataResponse;
    addExports(value?: GetMetadataResponse.ExportDefinition, index?: number): GetMetadataResponse.ExportDefinition;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): GetMetadataResponse.AsObject;
    static toObject(includeInstance: boolean, msg: GetMetadataResponse): GetMetadataResponse.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: GetMetadataResponse, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): GetMetadataResponse;
    static deserializeBinaryFromReader(message: GetMetadataResponse, reader: jspb.BinaryReader): GetMetadataResponse;
}

export namespace GetMetadataResponse {
    export type AsObject = {
        metadataList: Array<GetMetadataResponse.MetadataDefinition.AsObject>,
        parametersList: Array<GetMetadataResponse.SymbolDefinition.AsObject>,
        outputsList: Array<GetMetadataResponse.SymbolDefinition.AsObject>,
        exportsList: Array<GetMetadataResponse.ExportDefinition.AsObject>,
    }


    export class SymbolDefinition extends jspb.Message { 

        hasRange(): boolean;
        clearRange(): void;
        getRange(): Range | undefined;
        setRange(value?: Range): SymbolDefinition;
        getName(): string;
        setName(value: string): SymbolDefinition;

        hasType(): boolean;
        clearType(): void;
        getType(): GetMetadataResponse.TypeDefinition | undefined;
        setType(value?: GetMetadataResponse.TypeDefinition): SymbolDefinition;

        hasDescription(): boolean;
        clearDescription(): void;
        getDescription(): string | undefined;
        setDescription(value: string): SymbolDefinition;

        serializeBinary(): Uint8Array;
        toObject(includeInstance?: boolean): SymbolDefinition.AsObject;
        static toObject(includeInstance: boolean, msg: SymbolDefinition): SymbolDefinition.AsObject;
        static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
        static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
        static serializeBinaryToWriter(message: SymbolDefinition, writer: jspb.BinaryWriter): void;
        static deserializeBinary(bytes: Uint8Array): SymbolDefinition;
        static deserializeBinaryFromReader(message: SymbolDefinition, reader: jspb.BinaryReader): SymbolDefinition;
    }

    export namespace SymbolDefinition {
        export type AsObject = {
            range?: Range.AsObject,
            name: string,
            type?: GetMetadataResponse.TypeDefinition.AsObject,
            description?: string,
        }
    }

    export class ExportDefinition extends jspb.Message { 

        hasRange(): boolean;
        clearRange(): void;
        getRange(): Range | undefined;
        setRange(value?: Range): ExportDefinition;
        getName(): string;
        setName(value: string): ExportDefinition;
        getKind(): string;
        setKind(value: string): ExportDefinition;

        hasDescription(): boolean;
        clearDescription(): void;
        getDescription(): string | undefined;
        setDescription(value: string): ExportDefinition;

        serializeBinary(): Uint8Array;
        toObject(includeInstance?: boolean): ExportDefinition.AsObject;
        static toObject(includeInstance: boolean, msg: ExportDefinition): ExportDefinition.AsObject;
        static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
        static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
        static serializeBinaryToWriter(message: ExportDefinition, writer: jspb.BinaryWriter): void;
        static deserializeBinary(bytes: Uint8Array): ExportDefinition;
        static deserializeBinaryFromReader(message: ExportDefinition, reader: jspb.BinaryReader): ExportDefinition;
    }

    export namespace ExportDefinition {
        export type AsObject = {
            range?: Range.AsObject,
            name: string,
            kind: string,
            description?: string,
        }
    }

    export class TypeDefinition extends jspb.Message { 

        hasRange(): boolean;
        clearRange(): void;
        getRange(): Range | undefined;
        setRange(value?: Range): TypeDefinition;
        getName(): string;
        setName(value: string): TypeDefinition;

        serializeBinary(): Uint8Array;
        toObject(includeInstance?: boolean): TypeDefinition.AsObject;
        static toObject(includeInstance: boolean, msg: TypeDefinition): TypeDefinition.AsObject;
        static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
        static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
        static serializeBinaryToWriter(message: TypeDefinition, writer: jspb.BinaryWriter): void;
        static deserializeBinary(bytes: Uint8Array): TypeDefinition;
        static deserializeBinaryFromReader(message: TypeDefinition, reader: jspb.BinaryReader): TypeDefinition;
    }

    export namespace TypeDefinition {
        export type AsObject = {
            range?: Range.AsObject,
            name: string,
        }
    }

    export class MetadataDefinition extends jspb.Message { 
        getName(): string;
        setName(value: string): MetadataDefinition;
        getValue(): string;
        setValue(value: string): MetadataDefinition;

        serializeBinary(): Uint8Array;
        toObject(includeInstance?: boolean): MetadataDefinition.AsObject;
        static toObject(includeInstance: boolean, msg: MetadataDefinition): MetadataDefinition.AsObject;
        static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
        static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
        static serializeBinaryToWriter(message: MetadataDefinition, writer: jspb.BinaryWriter): void;
        static deserializeBinary(bytes: Uint8Array): MetadataDefinition;
        static deserializeBinaryFromReader(message: MetadataDefinition, reader: jspb.BinaryReader): MetadataDefinition;
    }

    export namespace MetadataDefinition {
        export type AsObject = {
            name: string,
            value: string,
        }
    }

}

export class GetDeploymentGraphRequest extends jspb.Message { 
    getPath(): string;
    setPath(value: string): GetDeploymentGraphRequest;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): GetDeploymentGraphRequest.AsObject;
    static toObject(includeInstance: boolean, msg: GetDeploymentGraphRequest): GetDeploymentGraphRequest.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: GetDeploymentGraphRequest, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): GetDeploymentGraphRequest;
    static deserializeBinaryFromReader(message: GetDeploymentGraphRequest, reader: jspb.BinaryReader): GetDeploymentGraphRequest;
}

export namespace GetDeploymentGraphRequest {
    export type AsObject = {
        path: string,
    }
}

export class GetDeploymentGraphResponse extends jspb.Message { 
    clearNodesList(): void;
    getNodesList(): Array<GetDeploymentGraphResponse.Node>;
    setNodesList(value: Array<GetDeploymentGraphResponse.Node>): GetDeploymentGraphResponse;
    addNodes(value?: GetDeploymentGraphResponse.Node, index?: number): GetDeploymentGraphResponse.Node;
    clearEdgesList(): void;
    getEdgesList(): Array<GetDeploymentGraphResponse.Edge>;
    setEdgesList(value: Array<GetDeploymentGraphResponse.Edge>): GetDeploymentGraphResponse;
    addEdges(value?: GetDeploymentGraphResponse.Edge, index?: number): GetDeploymentGraphResponse.Edge;

    serializeBinary(): Uint8Array;
    toObject(includeInstance?: boolean): GetDeploymentGraphResponse.AsObject;
    static toObject(includeInstance: boolean, msg: GetDeploymentGraphResponse): GetDeploymentGraphResponse.AsObject;
    static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
    static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
    static serializeBinaryToWriter(message: GetDeploymentGraphResponse, writer: jspb.BinaryWriter): void;
    static deserializeBinary(bytes: Uint8Array): GetDeploymentGraphResponse;
    static deserializeBinaryFromReader(message: GetDeploymentGraphResponse, reader: jspb.BinaryReader): GetDeploymentGraphResponse;
}

export namespace GetDeploymentGraphResponse {
    export type AsObject = {
        nodesList: Array<GetDeploymentGraphResponse.Node.AsObject>,
        edgesList: Array<GetDeploymentGraphResponse.Edge.AsObject>,
    }


    export class Node extends jspb.Message { 

        hasRange(): boolean;
        clearRange(): void;
        getRange(): Range | undefined;
        setRange(value?: Range): Node;
        getName(): string;
        setName(value: string): Node;
        getType(): string;
        setType(value: string): Node;
        getIsexisting(): boolean;
        setIsexisting(value: boolean): Node;

        hasRelativepath(): boolean;
        clearRelativepath(): void;
        getRelativepath(): string | undefined;
        setRelativepath(value: string): Node;

        serializeBinary(): Uint8Array;
        toObject(includeInstance?: boolean): Node.AsObject;
        static toObject(includeInstance: boolean, msg: Node): Node.AsObject;
        static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
        static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
        static serializeBinaryToWriter(message: Node, writer: jspb.BinaryWriter): void;
        static deserializeBinary(bytes: Uint8Array): Node;
        static deserializeBinaryFromReader(message: Node, reader: jspb.BinaryReader): Node;
    }

    export namespace Node {
        export type AsObject = {
            range?: Range.AsObject,
            name: string,
            type: string,
            isexisting: boolean,
            relativepath?: string,
        }
    }

    export class Edge extends jspb.Message { 
        getSource(): string;
        setSource(value: string): Edge;
        getTarget(): string;
        setTarget(value: string): Edge;

        serializeBinary(): Uint8Array;
        toObject(includeInstance?: boolean): Edge.AsObject;
        static toObject(includeInstance: boolean, msg: Edge): Edge.AsObject;
        static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
        static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
        static serializeBinaryToWriter(message: Edge, writer: jspb.BinaryWriter): void;
        static deserializeBinary(bytes: Uint8Array): Edge;
        static deserializeBinaryFromReader(message: Edge, reader: jspb.BinaryReader): Edge;
    }

    export namespace Edge {
        export type AsObject = {
            source: string,
            target: string,
        }
    }

}
