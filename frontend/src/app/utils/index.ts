export const formatDateString = (dateString: string): Date => {
  console.log('sin format', dateString)
  let formattedDateString = dateString
    .replace("a. m.", "AM")
    .replace("p. m.", "PM");
  console.warn(formattedDateString)

    return new Date(formattedDateString)
}

export const formatDateToString = (date: Date): string => {
  const str = date.toLocaleDateString();
  const year = date.getFullYear();
  const parts = str.split('/');
  const day = parts[0];
  const month = parts[1];

  const result =  `${year}-${month}-${day}`;
  console.log(result)
  return result;
}
