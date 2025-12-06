import { TeachingMethod } from '../enums/TeachingMethod';

/**
 * Converts acceptsOnline and acceptsInPerson boolean values to TeachingMethod enum
 * for UI display (radio buttons)
 */
export const convertBoolsToTeachingMethod = (
    acceptsOnline: boolean | undefined, 
    acceptsInPerson: boolean | undefined
): TeachingMethod => {
    if (acceptsOnline && !acceptsInPerson) {
        return TeachingMethod.ONLINE;
    }

    if (!acceptsOnline && acceptsInPerson) {
        return TeachingMethod.IN_PERSON;
    }

    return TeachingMethod.BOTH;
};

/**
 * Converts TeachingMethod enum to acceptsOnline and acceptsInPerson boolean values
 * for backend API calls
 */
export const convertTeachingMethodToBools = (method: TeachingMethod): {
    acceptsOnline: boolean;
    acceptsInPerson: boolean;
} => {
    switch (method) {
        case TeachingMethod.ONLINE:
            return { acceptsOnline: true, acceptsInPerson: false };
        case TeachingMethod.IN_PERSON:
            return { acceptsOnline: false, acceptsInPerson: true };
        case TeachingMethod.BOTH:
        default:
            return { acceptsOnline: true, acceptsInPerson: true };
    }
};