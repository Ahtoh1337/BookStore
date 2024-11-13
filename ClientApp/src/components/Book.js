import { useContext, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { UserContext } from "../App";
import Cookies from "js-cookie"
import PurchaseButton from "./PurchaseButton";

function Book() {
  const [data, setData] = useState({ state: "loading" });
  const bookId = parseInt(useParams()["bookId"]);
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function getGenre() {
      if (!bookId) {
        setData({ state: "fail", message: "Incorrect ID" });
        return;
      }

      const response = await userContext.authFetch(`api/books/${bookId}`);

      if (response.status !== 200) {
        setData({
          state: "fail",
          message: `${response.status} ${response.statusText}`,
        });
        return;
      }

      const body = await response.json();

      const authors = [];
      for (const authorId of body.authors) {
        const authorResponse = await userContext.authFetch(
          `api/authors/${authorId}`
        );
        if (response.status === 200) {
          const authorBody = await authorResponse.json();
          authors.push(authorBody);
        }
      }

      const genres = [];
      for (const genreId of body.genres) {
        const genreResponse = await userContext.authFetch(
          `api/genres/${genreId}`
        );
        if (response.status === 200) {
          const genreBody = await genreResponse.json();
          genres.push(genreBody);
        }
      }

      setData({
        book: { ...body, authors: authors, genres: genres },
        state: "ok",
      });
    }

    getGenre();
  }, []);

  return (
    <>
      {data.state == "loading" && <p>Loading...</p>}
      {data.state == "fail" && (
        <p>Loading failed. ({data.message ?? "Unknown reasons"})</p>
      )}
      {data.state == "ok" && (
        <>
          <h1>
            {data.book.title}{" "}
            <small className="fs-3 text-muted">
              "{data.book.publisher}" ({data.book.publishedYear})
            </small>
          </h1>
          <h3>
            Author(s):{" "}
            {data.book.authors.length === 0
              ? "Unknown"
              : data.book.authors.map((a, i, arr) => (
                  <span key={a.authorId}>
                    <Link to={`/authors/${a.authorId}`}>{a.name}</Link>
                    {i === arr.length - 1 ? "." : ", "}
                  </span>
                ))}
          </h3>
          <p>{data.book.description}</p>
          <h3>
            Genres:{" "}
            {data.book.genres.length === 0
              ? "Unknown"
              : data.book.genres.map((g, i, arr) => (
                  <span key={g.genreId}>
                    <Link to={`/genres/${g.genreId}`}>{g.name}</Link>
                    {i === arr.length - 1 ? "." : ", "}
                  </span>
                ))}
          </h3>
          <PurchaseButton
            book={data.book}
            userEmail={Cookies.get("userName")}
          />
        </>
      )}
    </>
  );
}

export default Book;
