const Hero = () => {
  return (
    <section
      id="top"
      className="relative overflow-hidden bg-white"
    >
      <div
        className="animate-fade-in absolute -top-32 -left-32 h-96 w-96 rounded-full bg-brand-teal/20 blur-3xl"
        style={{ animationDelay: "100ms" }}
      />
      <div
        className="animate-fade-in absolute -right-24 top-40 h-80 w-80 rounded-full bg-[#3bafac]/15 blur-3xl"
        style={{ animationDelay: "300ms" }}
      />

      <div className="relative mx-auto grid max-w-6xl items-center gap-16 px-6 py-20 md:grid-cols-2 md:px-10 md:py-28">
        <div>
          <span
            className="animate-fade-up inline-block rounded-full bg-brand-teal/10 px-4 py-1.5 font-body text-xs font-semibold tracking-wide text-brand-teal-light uppercase"
            style={{ animationDelay: "80ms" }}
          >
            For Bangladeshi Nurses &amp; IMGs
          </span>

          <h1
            className="animate-fade-up mt-6 font-heading text-4xl font-extrabold leading-[1.1] tracking-tight text-brand-ink sm:text-5xl lg:text-6xl"
            style={{ animationDelay: "200ms" }}
          >
            Your Bridge to a US{" "}
            <span className="bg-gradient-to-r from-[#3bafac] to-[#30cab3] bg-clip-text text-transparent">
              Healthcare Career
            </span>
          </h1>

          <p
            className="animate-fade-up mt-6 max-w-lg font-body text-lg leading-relaxed text-brand-slate"
            style={{ animationDelay: "340ms" }}
          >
            ZABiL Resources guides Bangladeshi nurses and physicians through
            every step of building a career in the United States — from
            training and licensing to visa sponsorship and permanent
            residency.
          </p>

          <div
            className="animate-fade-up mt-9 flex flex-wrap gap-4"
            style={{ animationDelay: "480ms" }}
          >
            <a
              href="#nurses"
              className="rounded-full bg-gradient-to-r from-[#3bafac] to-[#30cab3] px-6 py-3 font-body text-sm font-semibold text-white shadow-md shadow-brand-teal/30 transition-transform hover:scale-105"
            >
              For Nurses
            </a>
            <a
              href="#imgs"
              className="rounded-full border border-brand-teal/30 bg-white px-6 py-3 font-body text-sm font-semibold text-brand-ink transition-colors hover:border-brand-teal hover:text-brand-teal-light"
            >
              For IMGs
            </a>
          </div>

          <p
            className="animate-fade-up mt-8 font-body text-xs font-medium tracking-wide text-brand-slate/80"
            style={{ animationDelay: "600ms" }}
          >
            IN PARTNERSHIP WITH ADEX MEDICAL STAFFING
          </p>
        </div>

        <div
          className="animate-scale-in relative mx-auto w-full max-w-sm"
          style={{ animationDelay: "260ms" }}
        >
          <div className="absolute -bottom-6 -right-6 h-full w-full rounded-3xl bg-gradient-to-br from-[#3bafac] to-[#30cab3] opacity-90" />
          <div className="relative rounded-3xl border border-black/5 bg-white p-8 shadow-xl shadow-brand-ink/10">
            <div className="flex h-20 w-20 items-center justify-center rounded-2xl bg-gradient-to-br from-[#3bafac] to-[#30cab3]">
              <svg
                viewBox="0 0 24 24"
                fill="none"
                className="h-10 w-10 text-white"
                aria-hidden="true"
              >
                <path
                  d="M12 3v18M3 12h18"
                  stroke="currentColor"
                  strokeWidth="2.5"
                  strokeLinecap="round"
                />
              </svg>
            </div>

            <p className="mt-6 font-heading text-lg font-bold text-brand-ink">
              ZABiL Resources
            </p>
            <p className="mt-1 font-body text-sm text-brand-slate">
              Training · Licensing · Sponsorship
            </p>

            <ul className="mt-6 space-y-3 font-body text-sm text-brand-slate">
              <li className="flex items-center gap-2">
                <span className="h-1.5 w-1.5 rounded-full bg-brand-teal" />
                NCLEX-RN &amp; EB3 Pathway
              </li>
              <li className="flex items-center gap-2">
                <span className="h-1.5 w-1.5 rounded-full bg-brand-teal" />
                J1 Waiver &amp; H1B Sponsorship
              </li>
              <li className="flex items-center gap-2">
                <span className="h-1.5 w-1.5 rounded-full bg-brand-teal" />
                EB2 Green Card Processing
              </li>
            </ul>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Hero;
