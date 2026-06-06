import { useEffect, useState } from "react";
import {
	View,
	Text,
	TextInput,
	Pressable,
	ScrollView,
	ActivityIndicator,
} from "react-native";
import { SessionReviewService } from "../../services/SessionReviewService";

type Review = {
	rating: number;
	comment: string;
};

export default function SessionReview({
	sessionId,
}: {
	sessionId: number;
}) {
	const [reviews, setReviews] = useState<Review[]>([]);
	const [rating, setRating] = useState(5);
	const [comment, setComment] = useState("");
	const [submitting, setSubmitting] = useState(false);
	const [canReview, setCanReview] = useState(false);


	const loadReviews = async () => {
		const res = await SessionReviewService.getReviewsForSessionId(sessionId);
		if (!res.success) return;

		setReviews(res.reviews.reviews);
		setCanReview(res.reviews.canReview)
	};

	const submitReview = async () => {
		if (!canReview) return;
		if (!comment.trim()) return;

		setSubmitting(true);

		try {
			const res = await SessionReviewService.createReview(
				sessionId,
				rating,
				comment
			);

			if (res.success) {
				setComment("");
				setRating(5);
				await loadReviews();
			}
		} finally {
			setSubmitting(false);
		}
	};

	useEffect(() => {
		loadReviews();
	}, [sessionId]);

	return (
		<ScrollView style={{ padding: 16 }}>
		<Text style={{ fontSize: 22, fontWeight: "bold", marginBottom: 10 }}>
		Reviews
		</Text>

			{canReview && (
				<View
				style={{
					borderWidth: 1,
					borderColor: "#ccc",
					padding: 15,
					borderRadius: 8,
					marginBottom: 20,
				}}
				>
				<Text style={{ fontSize: 18, marginBottom: 10 }}>
				Leave a Review
				</Text>

				<View style={{ flexDirection: "row", marginBottom: 10 }}>
				{[1, 2, 3, 4, 5].map((value) => (
					<Pressable
						key={value}
						onPress={() => setRating(value)}
						style={{ marginRight: 8 }}
					>
						<Text
							style={{
								fontSize: 26,
								opacity: value <= rating ? 1 : 0.3,
							}}
						>
							⭐
						</Text>
					</Pressable>
				))}
				</View>

				<TextInput
				value={comment}
				onChangeText={setComment}
				placeholder="Write your review..."
				multiline
				style={{
					borderWidth: 1,
					borderColor: "#ccc",
					padding: 10,
					borderRadius: 6,
					minHeight: 80,
					marginBottom: 10,
					backgroundColor: "#fff",
				}}
				/>

				<Pressable
				onPress={submitReview}
				disabled={submitting || !comment.trim()}
				style={{
					backgroundColor:
						submitting || !comment.trim()
							? "#999"
							: "#007bff",
							padding: 10,
							borderRadius: 6,
							alignItems: "center",
				}}
				>
				{submitting ? (
					<ActivityIndicator color="#fff" />
				) : (
				<Text style={{ color: "#fff", fontWeight: "bold" }}>
				Submit Review
				</Text>
				)}
				</Pressable>
				</View>
			)}

			{!canReview && (
				<Text style={{ color: "gray", marginBottom: 10 }}>
				You can only leave a review if you have attended this session.
						</Text>
			)}

		{reviews.length === 0 ? (
			<Text>No reviews yet.</Text>
		) : (
		reviews.map((review, index) => (
			<View
			key={index}
			style={{
				borderWidth: 1,
				borderColor: "#ccc",
				padding: 10,
				marginBottom: 10,
				borderRadius: 6,
			}}
			>
			<Text>
				<Text>{"⭐".repeat(review.rating)}</Text>
				<Text style={{ opacity: 0.3 }}>{"⭐".repeat(5 - review.rating)}</Text>
			</Text>
			<Text>{review.comment}</Text>
			</View>
		))
		)}
		</ScrollView>
	);
}
