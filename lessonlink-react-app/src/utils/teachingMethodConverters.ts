import { TeachingMethod } from "./enums";

/**
 * Converts acceptsOnline and acceptsInPerson boolean values to TeachingMethod enum for UI display.
 */
export const convertBoolsToTeachingMethod = (
    acceptsOnline: boolean | undefined,
    acceptsInPerson: boolean | undefined
): TeachingMethod => {
    if (acceptsOnline && !acceptsInPerson) {
        return TeachingMethod.Online;
    }

    if (!acceptsOnline && acceptsInPerson) {
        return TeachingMethod.InPerson;
    }

    return TeachingMethod.Both;
};

/**
 * Converts TeachingMethod enum to acceptsOnline and acceptsInPerson boolean values for backend API calls.
 */
export const convertTeachingMethodToBools = (method: TeachingMethod): {
    acceptsOnline: boolean | undefined;
    acceptsInPerson: boolean | undefined;
} => {
    switch (method) {
        case TeachingMethod.Online:
            return { acceptsOnline: true, acceptsInPerson: undefined };
        case TeachingMethod.InPerson:
            return { acceptsOnline: undefined, acceptsInPerson: true };
        case TeachingMethod.Both:
        default:
            return { acceptsOnline: undefined, acceptsInPerson: undefined };
    }
};