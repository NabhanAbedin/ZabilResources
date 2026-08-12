const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const exchangeGoogleCode = async (code: string): Promise<string> => {
  const res = await fetch(`${API_BASE_URL}/api/auth/google/exchange`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(code), 
  });

  if (!res.ok) {
    throw new Error("Google sign-in failed");
  }

  return res.text(); 
}
