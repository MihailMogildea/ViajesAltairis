/* eslint-disable @next/next/no-img-element */
"use client";

import { useState, useCallback } from "react";

interface ImageCarouselProps {
  images: { url: string; alt_text: string }[];
  aspectRatio?: string;
}

export default function ImageCarousel({ images, aspectRatio = "aspect-[16/9]" }: ImageCarouselProps) {
  const [current, setCurrent] = useState(0);

  const prev = useCallback(() => setCurrent((c) => (c === 0 ? images.length - 1 : c - 1)), [images.length]);
  const next = useCallback(() => setCurrent((c) => (c === images.length - 1 ? 0 : c + 1)), [images.length]);

  if (images.length === 0) {
    return (
      <div className={`${aspectRatio} w-full rounded-xl bg-gray-200 flex items-center justify-center text-gray-400`}>
        No images
      </div>
    );
  }

  return (
    <div className="relative group">
      <div className={`${aspectRatio} w-full overflow-hidden rounded-xl bg-gray-100`}>
        <img
          src={images[current].url}
          alt={images[current].alt_text}
          className="h-full w-full object-cover transition-opacity duration-300"
        />
      </div>
      {images.length > 1 && (
        <>
          <button
            onClick={prev}
            className="absolute left-2 top-1/2 -translate-y-1/2 rounded-full bg-white/80 p-2 text-gray-800 shadow-md opacity-0 group-hover:opacity-100 transition-opacity hover:bg-white"
            aria-label="Previous image"
          >
            <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            onClick={next}
            className="absolute right-2 top-1/2 -translate-y-1/2 rounded-full bg-white/80 p-2 text-gray-800 shadow-md opacity-0 group-hover:opacity-100 transition-opacity hover:bg-white"
            aria-label="Next image"
          >
            <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
            </svg>
          </button>
          <div className="absolute bottom-3 left-1/2 -translate-x-1/2 flex gap-1.5">
            {images.map((_, i) => (
              <button
                key={i}
                onClick={() => setCurrent(i)}
                className={`h-2 w-2 rounded-full transition-colors ${
                  i === current ? "bg-white" : "bg-white/50"
                }`}
                aria-label={`Go to image ${i + 1}`}
              />
            ))}
          </div>
        </>
      )}
    </div>
  );
}
