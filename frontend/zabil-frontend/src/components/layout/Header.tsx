import { Link } from "react-router-dom";
import type { HeaderProps, NavLink } from "../../types/interfaces";

const navLinks: NavLink[] = [
  { label: "About Us", href: "#about" },
  { label: "For Nurses", href: "#nurses" },
  { label: "For IMGs", href: "#imgs" },
  { label: "Contact", href: "#contact" },
];

const Header = ({ isLoggedIn, onSignOut }: HeaderProps) => {
  return (
    <header className="sticky top-0 z-50 border-b border-black/5 bg-white/80 backdrop-blur-md">
      <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4 md:px-10">
        <a
          href="#top"
          className="font-heading text-xl font-extrabold tracking-tight text-brand-ink md:text-2xl"
        >
          ZAB<span className="text-brand-teal">i</span>L
          <span className="ml-2 hidden font-body text-xs font-medium tracking-wide text-brand-slate sm:inline">
            RESOURCES
          </span>
        </a>

        <nav className="hidden items-center gap-8 md:flex">
          {navLinks.map((link) => (
            <a
              key={link.href}
              href={link.href}
              className="font-body text-sm font-medium text-brand-slate transition-colors hover:text-brand-teal"
            >
              {link.label}
            </a>
          ))}
        </nav>

        <div className="flex items-center gap-4">
          {isLoggedIn ? (
            <button
              type="button"
              onClick={onSignOut}
              className="font-body text-sm font-medium text-brand-slate transition-colors hover:text-brand-teal"
            >
              Sign Out
            </button>
          ) : (
            <Link
              to="/login"
              className="font-body text-sm font-medium text-brand-slate transition-colors hover:text-brand-teal"
            >
              Sign In
            </Link>
          )}
          <a
            href="#contact"
            className="rounded-full bg-gradient-to-r from-[#3bafac] to-[#30cab3] px-5 py-2.5 font-body text-sm font-semibold text-white shadow-sm shadow-brand-teal/30 transition-transform hover:scale-105"
          >
            Get In Touch
          </a>
        </div>
      </div>
    </header>
  );
};

export default Header;
