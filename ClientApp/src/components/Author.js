import { useContext, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { UserContext } from "../App";
import BookTable from "./BookTable";

function Author() {
  const [data, setData] = useState({ state: "loading" });
  const authorId = parseInt(useParams()["authorId"]);
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function getGenre() {
      if (!authorId) {
        setData({ state: "fail", message: "Incorrect ID" });
        return;
      }

      const response = await userContext.authFetch(`api/authors/${authorId}`);

      if (response.status !== 200) {
        setData({
          state: "fail",
          message: `${response.status} ${response.statusText}`,
        });
        return;
      }

      const body = await response.json();

      const books = [];

      for (const bookId of body.authoredBooks) {
        const bookResponse = await userContext.authFetch(`api/books/${bookId}`);
        if (response.status === 200) {
          const bookBody = await bookResponse.json();
          books.push(bookBody);
        }
      }

      setData({ author: { ...body, authoredBooks: books }, state: "ok" });
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
          <h1>{data.author.name}</h1>
          <p>{data.author.description}</p>
          <h3>Authored books:</h3>
          <BookTable books={data.author.authoredBooks} />
        </>
      )}
    </>
  );
}

export default Author;
