import { Operation } from "apollo-link";
import { print } from "graphql";

export class Request {
  public operation: RequestOperation;
  constructor(public id: string, public type: string, operation: Operation) {
    if (operation != null)
      this.operation = new RequestOperation(operation);
    else
      this.operation = null;
  }
}

export class RequestOperation {
  constructor(operation: Operation) {
    this.variables = operation.variables;
    this.operationName = operation.operationName;
    this.extensions = operation.extensions;
    this.query = print(operation.query);
  }

  public query: string;
  public variables: Record<string, any>;
  public operationName: string;
  public extensions: Record<string, any>;
}
