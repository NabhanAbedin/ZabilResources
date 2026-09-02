import { Link } from "react-router-dom";

const ManagementConsolePage = () => {
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

      <main className="flex-1 px-6 py-12">
        <div className="mx-auto max-w-3xl">
          <h1 className="font-heading text-2xl font-bold text-brand-ink">
            Management Console
          </h1>
          <p className="mt-2 font-body text-sm text-brand-slate">
            Admin tools for managing Zabil Resources.
          </p>

          <div className="mt-8 grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Link
              to="/admin/posts/new"
              className="border border-black/10 bg-white p-6 transition-colors hover:border-brand-teal"
            >
              <h2 className="font-heading text-lg font-bold text-brand-ink">
                Create Post
              </h2>
              <p className="mt-1 font-body text-sm text-brand-slate">
                Publish a new post with optional photos or videos.
              </p>
            </Link>
          </div>
        </div>
      </main>
    </div>
  );
};

export default ManagementConsolePage;
