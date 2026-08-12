import { useEffect, useRef } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { exchangeGoogleCode } from "../api/authApi";
import { consumeStoredState } from "../lib/googleAuth";
import { saveToken } from "../lib/authToken";

const OAuthCallbackPage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const hasRun = useRef(false);

  const {
    mutate: exchangeCode,
    isPending,
    error,
  } = useMutation<string, Error, void>({
    mutationKey: ["exchangeGoogleCode"],
    mutationFn: async (): Promise<string> => {
      const googleError = searchParams.get("error");
      const code = searchParams.get("code");
      const returnedState = searchParams.get("state");
      const expectedState = consumeStoredState();

      if (googleError) {
        throw new Error("Google sign-in was cancelled or denied.");
      }
      if (!code) {
        throw new Error("No authorization code was returned by Google.");
      }
      if (!expectedState || returnedState !== expectedState) {
        throw new Error(
          "Could not verify this login attempt. Please try again.",
        );
      }

      return exchangeGoogleCode(code);
    },
    onSuccess: (jwt) => {
      saveToken(jwt);
      navigate("/", { replace: true });
    },
  });

  useEffect(() => {
    if (hasRun.current) return;
    hasRun.current = true;

    exchangeCode();
  }, [exchangeCode]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-b from-white to-brand-teal/5 px-6">
      <div className="w-full max-w-sm animate-fade-in rounded-2xl border border-black/5 bg-white p-8 text-center">
        {!error && (
          <>
            <div className="mx-auto h-8 w-8 animate-spin rounded-full border-2 border-brand-teal/30 border-t-brand-teal" />
            <p className="mt-4 font-body text-sm text-brand-slate">
              {isPending ? "Finishing sign-in…" : "Verifying login…"}
            </p>
          </>
        )}

        {error && (
          <>
            <p className="font-heading text-lg font-bold text-brand-ink">
              Sign-in failed
            </p>
            <p className="mt-2 font-body text-sm text-brand-slate">
              {error.message}
            </p>
            <Link
              to="/login"
              className="mt-6 inline-block rounded-full bg-gradient-to-r from-[#3bafac] to-[#30cab3] px-5 py-2.5 font-body text-sm font-semibold text-white shadow-sm shadow-brand-teal/30 transition-transform hover:scale-105"
            >
              Back to login
            </Link>
          </>
        )}
      </div>
    </div>
  );
};

export default OAuthCallbackPage;
