import { useContext, useEffect, useState } from "react";
import { UserContext } from "../App";
import BookTable from "./BookTable";
import { useParams } from "react-router-dom";

export default function PurchasedBooks() {
  const [data, setData] = useState({ books: [], state: "loading" });
  const userEmail = useParams()["email"];
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function func() {
      const purchaseResponse = await userContext.authFetch(
        `api/books/purchase/${userEmail}`
      );

      if (purchaseResponse.status !== 200) {
        setData({
          state: "fail",
          message: `${purchaseResponse.status} ${purchaseResponse.statusText}`,
        });
        return;
      }

      const purchaseBody = await purchaseResponse.json();
      if (purchaseBody.length === 0) {
        setData({ state: "fail", message: "User hasn't bought any books yet" });
        return;
      }

      const books = [];

      for (const p of purchaseBody) {
        const bookResponse = await userContext.authFetch(
          `api/books/${p.bookId}`
        );

        if (!bookResponse.ok) continue;

        books.push({ ...(await bookResponse.json()), rating: p.rating });
      }

      setData({ state: "ok", books: books });
    }

    func();
  }, []);

  return (
    <div>
      <h1>{userEmail}'s books</h1>
      {data.state == "loading" && <p>Loading...</p>}
      {data.state == "fail" && (
        <p>Loading failed. ({data.message ?? "Unknown reasons"})</p>
      )}
      {data.state == "ok" && (
        <BookTable books={data.books} withRating={true} withPrice={false} />
      )}
    </div>
  );
}
