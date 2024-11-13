import { useContext, useEffect, useState } from "react";
import { UserContext } from "../App";
import { Link } from "react-router-dom";

export default function AuthorList() {
  const [data, setData] = useState({ authors: [], state: "loading" });
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function func() {
      const response = await userContext.authFetch("api/authors");

      if (response.ok) {
        setData({ authors: await response.json(), state: "ok" });
      } else {
        setData({ state: "fail" });
      }
    }

    func();
  }, []);

  return (
    <div>
      <h1>Authors</h1>
      {data.state == "loading" && <p>Loading...</p>}
      {data.state == "fail" && <p>Loading failed.</p>}
      {data.state == "ok" && (
        <table className="table table-hover">
          <thead>
            <tr>
              <th>Name</th>
              <th>Books</th>
            </tr>
          </thead>
          <tbody>
            {data.authors.map((a) => (
              <tr key={a.authorId}>
                <td>
                  <Link to={`/authors/${a.authorId}`}>{a.name}</Link>
                </td>
                <td>{a.authoredBooks.length}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
