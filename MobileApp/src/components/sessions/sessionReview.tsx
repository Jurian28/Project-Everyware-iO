import { useEffect, useState } from "react";
import { SessionReviewService } from "../../services/SessionReviewService"


type Review = {
	stars: number;
	comment: string;
};

export default function SessionReview({sessionId}: {sessionId: number}) {

	const [reviews, setReviews] = useState<Review[]>([]);

	const loadReviews = async () => {
		const res = await SessionReviewService.getReviewsForSessionId(sessionId);
		if (res.success == false) return;
		const data = res.reviews;
		setReviews(data);
	};
	useEffect(() => {
		loadReviews();
	}, [sessionId]);


	return (
		<div>
		<h2>Reviews</h2>

		{reviews.length === 0 ? (
			<p>No reviews yet.</p>
		) : (
		reviews.map((review, index) => (
			<div
			key={index}
			style={{
				border: "1px solid #ccc",
				padding: "10px",
				marginBottom: "10px",
				borderRadius: "5px",
			}}
			>
			<div>{"⭐".repeat(review.stars)}</div>
			<p>{review.comment}</p>
			</div>
		))
		)}
		</div>
	)
}
