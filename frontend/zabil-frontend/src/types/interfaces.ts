export interface NavLink {
  label: string;
  href: string;
}

export interface HeaderProps {
  isLoggedIn: boolean;
  onSignOut: () => void;
}

export interface AboutCategory {
  heading: string;
  intro: string;
  points: string[];
}
