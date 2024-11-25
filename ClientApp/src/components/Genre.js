import { useContext, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { UserContext } from "../App";
import BookTable from "./BookTable";

function Genre() {
  const [data, setData] = useState({ state: "loading" });
  const genreId = parseInt(useParams()["genreId"]);
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function getGenre() {
      if (!genreId) {
        setData({ state: "fail", message: "Incorrect ID" });
        return;
      }

      const response = await userContext.authFetch(`api/genres/${genreId}`, {});

      if (response.status !== 200) {
        setData({
          state: "fail",
          message: `${response.status} ${response.statusText}`,
        });
        return;
      }

      const body = await response.json();

      const books = [];

      for (const bookId of body.books) {
        const bookResponse = await userContext.authFetch(`api/books/${bookId}`);
        if (response.status === 200) {
          const bookBody = await bookResponse.json();
          books.push(bookBody);
        }
      }

      setData({ genre: { ...body, books: books }, state: "ok" });
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
          <h1>{data.genre.name}</h1>
          <h3>Books in this genre: {data.genre.books.length}</h3>
          <BookTable books={data.genre.books}/>
        </>
      )}
    </>
  );
}

export default Genre;
