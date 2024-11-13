import { useContext, useEffect, useState } from "react";
import { UserContext } from "../App";
import { Link } from "react-router-dom";

export default function BookList() {
  const [data, setData] = useState({ books: [], state: "loading" });
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function func() {
      const response = await userContext.authFetch("api/books");

      if (response.ok) {
        setData({ books: await response.json(), state: "ok" });
      } else {
        setData({ state: "fail" });
      }
    }

    func();
  }, []);

  return (
    <div>
      <h1>Books</h1>
      {data.state == "loading" && <p>Loading...</p>}
      {data.state == "fail" && <p>Loading failed.</p>}
      {data.state == "ok" && (
        <table className="table table-hover">
          <thead>
            <tr>
              <th>Title</th>
              <th>Price</th>
              <th>Publisher</th>
            </tr>
          </thead>
          <tbody>
            {data.books.map((b) => (
              <tr key={b.bookId}>
                <td>
                  <Link to={`/books/${b.bookId}`}>{b.title}</Link>
                </td>
                <td>{b.price} UAH</td>
                <td>
                  "{b.publisher}" ({b.publishedYear})
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
