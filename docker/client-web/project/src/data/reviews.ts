import type { Review } from "@/types";

export const reviews: Review[] = [
  {
    id: 1, hotel_id: 1, user_name: "Juan M.", rating: 5,
    title: "Exceptional stay, will come back!",
    comment: "From the moment we arrived, the staff made us feel welcome. The double room was spacious and beautifully decorated with stunning views of the bay. Breakfast was outstanding with a great selection of local and international options. The location is perfect, walking distance to the old town and the beach. Highly recommended for couples looking for a luxury getaway in Palma.",
    created_at: "2026-01-15",
    response: { id: 1, review_id: 1, responder_name: "Marta Serra, Hotel Director", comment: "Dear Juan, thank you so much for your wonderful review! We are delighted to hear that you enjoyed your stay with us. We look forward to welcoming you back to Hotel Altairis Palma soon.", created_at: "2026-01-16" },
  },
  {
    id: 2, hotel_id: 2, user_name: "Emma W.", rating: 2,
    title: "Disappointing experience, not as advertised",
    comment: "The room was much smaller than expected and the air conditioning was not working properly during my stay. I reported it to reception twice but it was never fixed. The bathroom needed updating and the shower pressure was very low. The only positive was the friendly reception staff who tried their best to help.",
    created_at: "2026-01-20",
    response: { id: 2, review_id: 2, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Emma, we sincerely apologise for the inconvenience during your stay. We take all feedback seriously and have already addressed the air conditioning issue and scheduled bathroom renovations.", created_at: "2026-01-21" },
  },
  {
    id: 3, hotel_id: 3, user_name: "Juan M.", rating: 4,
    title: "Great beach hotel",
    comment: "Hotel Playa de Palma has an excellent location right on the beachfront with easy access to shops and restaurants. The pool area was really enjoyable. Half board dinner options were varied and well prepared. The only downside was some noise from nearby bars in the evening.",
    created_at: "2026-01-25",
    response: { id: 3, review_id: 3, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Juan, thank you for your kind review. We are glad you enjoyed the beach location and our pool facilities. We are looking into soundproofing options for the rooms facing the entertainment area.", created_at: "2026-01-26" },
  },
  {
    id: 4, hotel_id: 4, user_name: "Emma W.", rating: 3,
    title: "Decent budget option",
    comment: "The Casco Antiguo hotel benefits from a fantastic location right in the heart of the old town, within walking distance of the cathedral and main attractions. The twin room was basic but clean. Breakfast was average with a limited selection. For the price point it is a fair deal.",
    created_at: "2026-01-28",
    response: { id: 4, review_id: 4, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Emma, thank you for staying with us. We are pleased you appreciated our central location. We are working on expanding the breakfast menu.", created_at: "2026-01-29" },
  },
  {
    id: 5, hotel_id: 5, user_name: "Roberto G.", rating: 5,
    title: "Perfect Christmas getaway",
    comment: "We spent a wonderful Christmas week at Hotel Bellver Park and it exceeded all our expectations. The suite had amazing views of Bellver Castle. The restaurant served outstanding food with a special Christmas Eve dinner that was truly memorable. The staff were incredibly family friendly.",
    created_at: "2026-01-05",
    response: { id: 5, review_id: 5, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Roberto, we are thrilled that you had such a special Christmas at Hotel Bellver Park. Our team takes enormous pride in creating a festive atmosphere.", created_at: "2026-01-06" },
  },
  {
    id: 6, hotel_id: 6, user_name: "Juan M.", rating: 4,
    title: "Luxury with minor flaws",
    comment: "Hotel Marina Palma is a stunning five-star property with breathtaking views over the marina. The suite was beautifully appointed and the spa facilities were world class. The all-inclusive dining was excellent. The only issue was one evening when room service took over an hour to arrive.",
    created_at: "2026-02-01",
    response: { id: 6, review_id: 6, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Juan, thank you for your generous review. We sincerely apologise for the delay with room service and have reviewed our staffing during peak hours.", created_at: "2026-02-02" },
  },
  {
    id: 7, hotel_id: 7, user_name: "Oliver K.", rating: 3,
    title: "Good location, average hotel",
    comment: "The views of Palma Cathedral from my room were absolutely beautiful. The location is perfect for exploring the historic centre. However, the room felt a bit dated with worn furniture. Breakfast was good quality but the selection was somewhat limited.",
    created_at: "2026-02-05",
    response: { id: 7, review_id: 7, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Oliver, we are glad the cathedral views lived up to expectations. A full renovation programme is scheduled for the coming months.", created_at: "2026-02-06" },
  },
  {
    id: 8, hotel_id: 8, user_name: "Carmen L.", rating: 1,
    title: "Terrible experience",
    comment: "This was by far the worst hotel stay I have ever had. Our double room faced the main street and the noise was unbearable. The bathroom was not properly cleaned when we arrived. The night shift receptionist was extremely rude when I tried to raise these issues.",
    created_at: "2026-02-08",
    response: { id: 8, review_id: 8, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Carmen, we are deeply sorry to read about your experience. This falls far below our standards. We have launched an immediate investigation into the cleanliness protocols and staff conduct.", created_at: "2026-02-09" },
  },
  {
    id: 9, hotel_id: 9, user_name: "Emma W.", rating: 4,
    title: "Lovely neighbourhood hotel",
    comment: "Hotel Santa Catalina is perfectly situated in the trendy Santa Catalina neighbourhood with fantastic restaurants and boutiques just steps away. The junior suite was modern and comfortable with a lovely balcony. The half board dinner was genuinely good.",
    created_at: "2026-02-10",
    response: { id: 9, review_id: 9, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Emma, thank you for your lovely review. We are thrilled you enjoyed the neighbourhood. Your suggestion about a wellness area is noted.", created_at: "2026-02-11" },
  },
  {
    id: 10, hotel_id: 10, user_name: "Friedrich H.", rating: 5,
    title: "Best hotel in Mallorca",
    comment: "Hotel Son Vida Palace is without question the finest hotel on the island. The palatial estate and grounds are magnificent. The deluxe room was extraordinary with every luxury imaginable. The Michelin-quality dining was an experience in itself. The service was impeccable throughout.",
    created_at: "2026-02-14",
    response: { id: 10, review_id: 10, responder_name: "Antoni Barceló, Regional Manager", comment: "Dear Friedrich, what an honour to receive such a glowing review. Our team takes enormous pride in delivering exceptional service.", created_at: "2026-02-15" },
  },
];

export function getReviewsForHotel(hotelId: number): Review[] {
  return reviews.filter((r) => r.hotel_id === hotelId);
}

export function getAverageRating(hotelId: number): number {
  const hotelReviews = getReviewsForHotel(hotelId);
  if (hotelReviews.length === 0) return 0;
  return hotelReviews.reduce((sum, r) => sum + r.rating, 0) / hotelReviews.length;
}
