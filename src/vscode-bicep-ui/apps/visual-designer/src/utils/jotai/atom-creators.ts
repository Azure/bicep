import { atom } from "jotai";

export function atomWithNullCheck<Value>(initialValue?: Value | null) {
  const nullable = atom(initialValue);

  return atom(
    (get) => {
      const value = get(nullable);

      if (value === undefined || value === null) {
        throw new Error("Value is null or undefined");
      }

      return value;
    },
    (_, set, value: Value) => set(nullable, value),
  );
}
