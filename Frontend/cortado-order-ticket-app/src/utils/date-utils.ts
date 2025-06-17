export function formatDate(dateString: string, locale: string): string {
  const date = new Date(dateString);
  // Function to add ordinal suffix (st, nd, rd, th)
  const getOrdinalSuffix = (day: number): string => {
    if (day > 3 && day < 21) return "th";
    switch (day % 10) {
      case 1:
        return "st";
      case 2:
        return "nd";
      case 3:
        return "rd";
      default:
        return "th";
    }
  };

  const day = date.getDate();
  const suffix = getOrdinalSuffix(day);

  const formattedDate = date
    .toLocaleDateString(locale, {
      year: "numeric",
      month: "long",
      day: "numeric",
    })
    .replace(/(\d+)/, `$1${suffix}`);

  return formattedDate;
}

export function formatTime(time24: string): string {
  // Parse the hours and minutes
  const [hours, minutes] = time24.split(":").map(Number);

  // Determine period (AM/PM)
  const period = hours >= 12 ? "PM" : "AM";

  // Convert to 12-hour format
  let hours12 = hours % 12;
  hours12 = hours12 === 0 ? 12 : hours12;

  // Format the time string with padding for minutes
  return `${hours12}:${minutes.toString().padStart(2, "0")} ${period}`;
}

export function encryptTimestamp(timestamp: number): string {
  // Convert timestamp to base64 and add some obfuscation
  const encoded = Buffer.from(timestamp.toString()).toString("base64");
  return encoded.split("").reverse().join("");
}

export function decryptTimestamp(encrypted: string): number {
  // Reverse the obfuscation and decode base64
  const decoded = Buffer.from(encrypted.split("").reverse().join(""), "base64");
  return parseInt(decoded.toString());
}
