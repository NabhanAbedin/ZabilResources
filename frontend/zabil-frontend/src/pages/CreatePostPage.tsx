import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { createPost } from "../api/postApi";
import MediaPicker from "../components/posts/MediaPicker";
import type { CreatePostFormValues } from "../types/interfaces";

const MAX_MEDIA_FILES = 3;

const emptyForm: CreatePostFormValues = {
  title: "",
  message: "",
  category: "Unclassified",
  status: "Draft",
};

interface CreatePostSubmission {
  values: CreatePostFormValues;
  media: File[];
}

const CreatePostPage = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState<CreatePostFormValues>(emptyForm);
  const [mediaFiles, setMediaFiles] = useState<File[]>([]);

  const {
    mutate: submitPost,
    isPending,
    error,
  } = useMutation<void, Error, CreatePostSubmission>({
    mutationKey: ["createPost"],
    mutationFn: ({ values, media }) => createPost(values, media),
    onSuccess: () => {
      navigate("/");
    },
  });

  const onChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>,
  ) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const onAddMedia = (files: File[]) => {
    setMediaFiles((prev) => [...prev, ...files].slice(0, MAX_MEDIA_FILES));
  };

  const onRemoveMedia = (index: number) => {
    setMediaFiles((prev) => prev.filter((_, i) => i !== index));
  };

  const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    submitPost({ values: form, media: mediaFiles });
  };

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

      <main className="flex flex-1 items-center justify-center px-6 py-12">
        <form
          onSubmit={onSubmit}
          className="w-full max-w-lg animate-fade-up rounded-2xl border border-black/5 bg-white p-8 shadow-xl shadow-brand-ink/5"
        >
          <h1 className="font-heading text-2xl font-bold text-brand-ink">
            Create Post
          </h1>

          <label className="mt-6 block font-body text-sm font-medium text-brand-ink">
            Title
            <input
              type="text"
              name="title"
              value={form.title}
              onChange={onChange}
              className="mt-1 w-full rounded-lg border border-black/10 px-3 py-2 font-body text-sm text-brand-ink focus:border-brand-teal focus:outline-none"
            />
          </label>

          <label className="mt-4 block font-body text-sm font-medium text-brand-ink">
            Message
            <textarea
              name="message"
              value={form.message}
              onChange={onChange}
              required
              rows={5}
              className="mt-1 w-full rounded-lg border border-black/10 px-3 py-2 font-body text-sm text-brand-ink focus:border-brand-teal focus:outline-none"
            />
          </label>

          <label className="mt-4 block font-body text-sm font-medium text-brand-ink">
            Category
            <select
              name="category"
              value={form.category}
              onChange={onChange}
              className="mt-1 w-full rounded-lg border border-black/10 px-3 py-2 font-body text-sm text-brand-ink focus:border-brand-teal focus:outline-none"
            >
              <option value="ContentFeed">Content Feed</option>
              <option value="SuccessStory">Success Story</option>
              <option value="Repost">Repost</option>
              <option value="Unclassified">Unclassified</option>
            </select>
          </label>

          <label className="mt-4 block font-body text-sm font-medium text-brand-ink">
            Status
            <select
              name="status"
              value={form.status}
              onChange={onChange}
              className="mt-1 w-full rounded-lg border border-black/10 px-3 py-2 font-body text-sm text-brand-ink focus:border-brand-teal focus:outline-none"
            >
              <option value="Draft">Draft</option>
              <option value="PendingReview">Pending Review</option>
              <option value="Published">Published</option>
              <option value="Hidden">Hidden</option>
              <option value="Removed">Removed</option>
            </select>
          </label>

          <MediaPicker
            files={mediaFiles}
            maxFiles={MAX_MEDIA_FILES}
            onAdd={onAddMedia}
            onRemove={onRemoveMedia}
          />

          {error && (
            <p className="mt-4 font-body text-sm text-red-600">
              {error.message}
            </p>
          )}

          <button
            type="submit"
            disabled={isPending}
            className="mt-6 w-full rounded-full bg-gradient-to-r from-[#3bafac] to-[#30cab3] px-5 py-2.5 font-body text-sm font-semibold text-white shadow-sm shadow-brand-teal/30 transition-transform hover:scale-105 disabled:cursor-not-allowed disabled:opacity-60 disabled:hover:scale-100"
          >
            {isPending ? "Creating…" : "Create Post"}
          </button>
        </form>
      </main>
    </div>
  );
};

export default CreatePostPage;
