const Footer = () => {
  return (
    <footer
      id="contact"
      className="text-white"
      style={{ background: "linear-gradient(135deg, #3bafac, #30cab3)" }}
    >
      <div className="mx-auto flex max-w-6xl flex-col items-center gap-4 px-6 py-12 text-center md:px-10">
        <span className="font-heading text-2xl font-extrabold tracking-tight">
          ZABiL Resources
        </span>
        <p className="font-body text-sm text-white/90">
          Your trusted bridge to a successful healthcare career in the United
          States.
        </p>

        <div className="mt-2 flex flex-col items-center gap-2 font-body text-sm text-white/90 sm:flex-row sm:gap-6">
          <a
            href="tel:+13135757711"
            className="transition-opacity hover:opacity-80"
          >
            +1 (313) 575-7711
          </a>
          <span className="hidden h-1 w-1 rounded-full bg-white/50 sm:block" />
          <a
            href="mailto:mzabedin@zabilresources.com"
            className="transition-opacity hover:opacity-80"
          >
            mzabedin@zabilresources.com
          </a>
        </div>

        <p className="mt-4 font-body text-xs text-white/75">
          © 2025 ZABiL. All rights reserved.
        </p>
      </div>
    </footer>
  );
};

export default Footer;
