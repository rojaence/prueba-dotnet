export const formatDateString = (dateString: string): Date => {
  console.log('sin format', dateString)
  let formattedDateString = dateString
    .replace("a. m.", "AM")
    .replace("p. m.", "PM");
  console.warn(formattedDateString)

    return new Date(formattedDateString)
}
