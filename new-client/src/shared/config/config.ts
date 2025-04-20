const getEnvironmentVariable = (key: string) => {
  return import.meta.env[key]
}

export const ApiUrl = getEnvironmentVariable("VITE_API_URL")
