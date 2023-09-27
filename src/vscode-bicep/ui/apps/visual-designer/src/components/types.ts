export type Transient<TProps> = {
  [K in keyof TProps & string as `$${K}`]: TProps[K];
};
