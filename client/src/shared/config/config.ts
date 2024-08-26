const getEnvironmentVariable = (key: string) => {
  if (import.meta.env[key] === undefined) {
    throw new Error(`Env variable ${ key } is required`);
  }
  return import.meta.env[key] || "";
}

export const ApiUrl = getEnvironmentVariable("VITE_API_URL")
