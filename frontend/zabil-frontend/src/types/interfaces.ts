import type { ReactNode } from "react";

export interface NavLink {
  label: string;
  href: string;
}

export interface HeaderProps {
  isLoggedIn: boolean;
  isAdmin: boolean;
  onSignOut: () => void;
}

export interface AboutCategory {
  heading: string;
  intro: string;
  points: string[];
}

export interface DecodedAuthClaims {
  userId: string;
  Email: string;
  Role: "User" | "Admin";
  exp: number;
}

export interface RequireAdminProps {
  children: ReactNode;
}

export type SessionEndedReason = "expired" | "unauthorized" | "signed-out";

export interface AuthSnapshot {
  isLoggedIn: boolean;
  isAdmin: boolean;
  email: string | null;
  endedReason: SessionEndedReason | null;
}

export type PostCategory =
  | "ContentFeed"
  | "SuccessStory"
  | "Repost"
  | "Unclassified";

export type PostStatus =
  | "Draft"
  | "PendingReview"
  | "Published"
  | "Hidden"
  | "Removed";

export interface CreatePostFormValues {
  title?: string;
  message: string;
  category: PostCategory;
  status: PostStatus;
}

export interface MediaPickerProps {
  files: File[];
  maxFiles: number;
  onAdd: (files: File[]) => void;
  onRemove: (index: number) => void;
}
