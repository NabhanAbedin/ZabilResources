import type { CreatePostFormValues } from "../types/interfaces";
import { getToken } from "../lib/authToken";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const createPost = async (
  values: CreatePostFormValues,
  media: File[],
): Promise<void> => {
  // TODO: API call goes here.
  const formData = new FormData();
  formData.append("title", values.title ?? "");
  formData.append("message", values.message);
  formData.append("category", values.category);
  formData.append("status", values.status);
  media.forEach((file) => formData.append("mediaFiles", file));
  
  const res = await fetch(`${API_BASE_URL}/api/posts`, {
   method: "POST",
   headers: { Authorization: `Bearer ${getToken()}` },
   body: formData,
   });
   if (!res.ok) {
     throw new Error("Failed to create post");
  }
   return;
 
};
