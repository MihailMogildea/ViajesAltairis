export interface ReviewDto {
  id: number;
  reservationId: number;
  userId: number;
  hotelId: number;
  rating: number;
  title: string | null;
  comment: string | null;
  visible: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ReviewResponseDto {
  id: number;
  reviewId: number;
  userId: number;
  comment: string;
  createdAt: string;
}
