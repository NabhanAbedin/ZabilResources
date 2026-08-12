import { Link } from "react-router-dom";
import GoogleIcon from "../components/auth/GoogleIcon";
import { redirectToGoogleLogin } from "../lib/googleAuth";

const LoginPage = () => {
  return (
    <div className="flex min-h-screen flex-col bg-gradient-to-b from-white to-brand-teal/5">
      <header className="px-6 py-6 md:px-10">
        <Link
          to="/"
          className="font-heading text-xl font-extrabold tracking-tight text-brand-ink md:text-2xl"
        >
          ZAB<span className="text-brand-teal">i</span>L
        </Link>
      </header>

      <main className="flex flex-1 items-center justify-center px-6">
        <div className="w-full max-w-sm animate-fade-up rounded-2xl border border-black/5 bg-white p-8 shadow-xl shadow-brand-ink/5">
          <h1 className="font-heading text-2xl font-bold text-brand-ink">
            Welcome back
          </h1>
          <p className="mt-2 font-body text-sm text-brand-slate">
            Sign in to access your Zabil Resources dashboard.
          </p>

          <button
            type="button"
            onClick={redirectToGoogleLogin}
            className="mt-8 flex w-full items-center justify-center gap-3 rounded-full border border-black/10 bg-white px-5 py-3 font-body text-sm font-semibold text-brand-ink shadow-sm transition-colors hover:bg-black/[0.03]"
          >
            <GoogleIcon />
            Continue with Google
          </button>

          <p className="mt-6 text-center font-body text-xs text-brand-slate">
            By continuing you agree to our Terms of Service and Privacy
            Policy.
          </p>
        </div>
      </main>
    </div>
  );
};

export default LoginPage;
