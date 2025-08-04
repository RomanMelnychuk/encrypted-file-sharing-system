import { parseISO, format } from 'date-fns';

export const convertToLocalDateTime = (utcDateTimeString: string) => {
    const isoStringWithZ = utcDateTimeString.endsWith('Z') ? utcDateTimeString : `${utcDateTimeString}Z`;

    const parsedDate = parseISO(isoStringWithZ);

    return format(parsedDate, 'dd-MM-yyyy HH:mm:ss');
};
