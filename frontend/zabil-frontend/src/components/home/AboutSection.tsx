import type { AboutCategory } from "../../types/interfaces";

const categories: AboutCategory[] = [
  {
    heading: "For Bangladeshi Nurses",
    intro:
      "ZABiL Resources is your trusted bridge to a successful nursing career in the United States. We provide end-to-end support — from NCLEX-RN training to licensing and EB3 green card sponsorship — in partnership with ADEX Medical Staffing.",
    points: [
      "Online NCLEX-RN Training for Bangladeshi Nurses",
      "US State Board of Nursing Selection & Licensing Guidance",
      "US Job Placement with EB3 Visa Sponsorship via ADEX Medical Staffing",
    ],
  },
  {
    heading: "For Bangladeshi IMGs (International Medical Graduates)",
    intro:
      "We support BD IMGs in US Residency PGY1-3 in the US on J1 visas through H1B sponsorship, J1 waiver assistance, and EB2 green card pathways via ADEX Medical Staffing.",
    points: [
      "J1 Waiver and H1B Sponsorship",
      "US MD Job Placement Support",
      "EB2 Green Card Processing",
    ],
  },
];

const sectionIds = ["nurses", "imgs"];

const AboutSection = () => {
  return (
    <section id="about" className="bg-[#f7fbfa] py-20 md:py-28">
      <div className="mx-auto max-w-6xl px-6 md:px-10">
        <div className="mx-auto max-w-2xl text-center">
          <span className="font-body text-xs font-semibold uppercase tracking-wide text-brand-teal-light">
            About Us
          </span>
          <h2 className="mt-3 font-heading text-3xl font-extrabold tracking-tight text-brand-ink sm:text-4xl">
            Guiding Your Path to Practice in the US
          </h2>
        </div>

        <div className="mt-14 grid gap-8 md:grid-cols-2">
          {categories.map((category, index) => (
            <article
              key={category.heading}
              id={sectionIds[index]}
              className="scroll-mt-24 rounded-3xl border border-black/5 bg-white p-8 shadow-sm shadow-brand-ink/5"
            >
              <h3 className="font-heading text-xl font-bold text-brand-ink">
                {category.heading}
              </h3>
              <p className="mt-4 font-body text-sm leading-relaxed text-brand-slate">
                {category.intro}
              </p>

              <ul className="mt-6 space-y-3">
                {category.points.map((point) => (
                  <li key={point} className="flex items-start gap-3">
                    <svg
                      viewBox="0 0 20 20"
                      fill="none"
                      className="mt-0.5 h-5 w-5 flex-shrink-0 text-brand-teal"
                      aria-hidden="true"
                    >
                      <circle cx="10" cy="10" r="10" fill="currentColor" opacity="0.12" />
                      <path
                        d="m6 10.5 2.5 2.5L14 7.5"
                        stroke="currentColor"
                        strokeWidth="1.8"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                    </svg>
                    <span className="font-body text-sm text-brand-slate">
                      {point}
                    </span>
                  </li>
                ))}
              </ul>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
};

export default AboutSection;
