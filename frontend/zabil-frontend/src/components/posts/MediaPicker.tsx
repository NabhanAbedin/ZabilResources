import { useEffect, useMemo } from "react";
import type { MediaPickerProps } from "../../types/interfaces";

const MediaPicker = ({ files, maxFiles, onAdd, onRemove }: MediaPickerProps) => {
  const previewUrls = useMemo(
    () => files.map((file) => URL.createObjectURL(file)),
    [files],
  );

  useEffect(() => {
    return () => {
      previewUrls.forEach((url) => URL.revokeObjectURL(url));
    };
  }, [previewUrls]);

  const onFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selected = Array.from(e.target.files ?? []);
    const remainingSlots = maxFiles - files.length;
    onAdd(selected.slice(0, remainingSlots));
    e.target.value = "";
  };

  return (
    <div className="mt-4">
      <span className="block font-body text-sm font-medium text-brand-ink">
        Photos / Videos ({files.length}/{maxFiles})
      </span>

      {files.length > 0 && (
        <div className="mt-2 grid grid-cols-3 gap-2">
          {files.map((file, index) => (
            <div
              key={`${file.name}-${index}`}
              className="relative overflow-hidden rounded-lg border border-black/10"
            >
              {file.type.startsWith("video/") ? (
                <video src={previewUrls[index]} className="h-24 w-full object-cover" muted />
              ) : (
                <img
                  src={previewUrls[index]}
                  alt={file.name}
                  className="h-24 w-full object-cover"
                />
              )}
              <button
                type="button"
                onClick={() => onRemove(index)}
                aria-label={`Remove ${file.name}`}
                className="absolute right-1 top-1 flex h-5 w-5 items-center justify-center rounded-full bg-black/60 text-xs text-white"
              >
                ×
              </button>
            </div>
          ))}
        </div>
      )}

      {files.length < maxFiles && (
        <label className="mt-2 inline-block cursor-pointer rounded-lg border border-dashed border-black/20 px-3 py-2 font-body text-sm text-brand-slate transition-colors hover:border-brand-teal hover:text-brand-teal">
          Add photo or video
          <input
            type="file"
            accept="image/*,video/*"
            multiple
            onChange={onFileInputChange}
            className="hidden"
          />
        </label>
      )}
    </div>
  );
};

export default MediaPicker;
