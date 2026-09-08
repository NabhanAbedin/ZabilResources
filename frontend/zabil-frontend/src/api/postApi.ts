import type { CreatePostFormValues } from "../types/interfaces";
import { apiFetch } from "./http";

export const createPost = async (
  values: CreatePostFormValues,
  media: File[],
): Promise<void> => {
  const formData = new FormData();
  formData.append("title", values.title ?? "");
  formData.append("message", values.message);
  formData.append("category", values.category);
  formData.append("status", values.status);
  media.forEach((file) => formData.append("mediaFiles", file));

  const res = await apiFetch("/api/posts", {
    method: "POST",
    body: formData,
  });

  if (!res.ok) {
    throw new Error("Failed to create post");
  }
};
