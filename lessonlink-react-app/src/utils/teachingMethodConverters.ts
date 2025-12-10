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
 * Converts TeachingMethod enum to acceptsOnline and acceptsInPerson boolean values for filters.
 */
export const convertTeachingMethodToFilterBools = (method: TeachingMethod): {
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

/**
 * Converts TeachingMethod enum to explicit acceptsOnline and acceptsInPerson boolean values for registration and profile updates.
 */
export const convertTeachingMethodToExplicitBools = (method: TeachingMethod): {
    acceptsOnline: boolean;
    acceptsInPerson: boolean;
} => {
    switch (method) {
        case TeachingMethod.Online:
            return { acceptsOnline: true, acceptsInPerson: false };
        case TeachingMethod.InPerson:
            return { acceptsOnline: false, acceptsInPerson: true };
        case TeachingMethod.Both:
        default:
            return { acceptsOnline: true, acceptsInPerson: true };
    }
};