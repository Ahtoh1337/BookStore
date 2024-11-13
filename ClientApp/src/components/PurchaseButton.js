import { useContext, useState, useEffect } from "react";
import { UserContext } from "../App";
import { Link } from "react-router-dom";

function PurchaseButton({ book, userEmail }) {
  /// if unauthorized - message offering to login or register
  /// if not purchased - purchase button
  /// if purchased - rate button
  /// state = loading | fail | unauthorized | buy | rate
  const [data, setData] = useState({ rating: null, state: "loading" });
  const userContext = useContext(UserContext);

  useEffect(() => {
    getPurchase();
  }, []);

  async function getPurchase() {
    if (!userEmail) {
      setData({ state: "unauthorized" });
      return;
    }

    const response = await userContext.authFetch(
      `api/books/purchase/${userEmail}/${book.bookId}`
    );

    if (response.status === 404) {
      setData({ state: "buy" });
      return;
    }

    if (response.status !== 200) {
      setData({ state: "fail", message: response.statusText });
      return;
    }

    const body = await response.json();

    setData({ state: "rate", rating: body?.rating });
  }

  async function buyBook() {
    if (!userEmail) {
      setData({ state: "unauthorized" });
      return;
    }

    const response = await userContext.authFetch("api/books/purchase", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        userEmail: userEmail,
        bookId: book.bookId,
      }),
    });

    if (response.status !== 200) {
      setData({ state: "fail", message: response.statusText });
      return;
    }

    const body = await response.json();

    setData({ state: "rate", rating: body?.rating });
  }

  async function rateBook(e) {
    const rating = e.currentTarget.value
    setData({...data, rating: rating})

    if (!userEmail) {
      setData({ state: "unauthorized" });
      return;
    }

    const response = await userContext.authFetch("api/books/purchase", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        userEmail: userEmail,
        bookId: book.bookId,
        rating: rating,
      }),
    });

    if (response.status !== 200) {
        setData({ state: "fail", message: response.statusText });
      return;
    }
  }

  return (
    <div>
      {data.state == "loading" && <p>Loading...</p>}
      {data.state == "fail" && (
        <p>Loading failed. ({data.message ?? "Unknown reasons"})</p>
      )}
      {data.state == "unauthorized" && (
        <>
          <p>
            <Link to="/sign-in" className="text-dark fw-bold">
              Sign In
            </Link>{" "}
            or{" "}
            <Link to="/sign-up" className="text-dark fw-bold">
              Sign Up
            </Link>{" "}
            to purchase this book for <b>{book.price} UAH</b>
          </p>
        </>
      )}
      {data.state == "buy" && (
        <button className="btn btn-success" onClick={buyBook}>
          {book.price} UAH
        </button>
      )}
      {data.state == "rate" && (
        <>
          <input onChange={rateBook}
            type="range"
            min="1"
            max="5"
            step="1"
            value={data?.rating ?? "1"}
          />
          <span>{" "}{data?.rating ? `Your rating: ${data?.rating}` : "Rate this book!"}</span>
        </>
      )}
    </div>
  );
}

export default PurchaseButton;
