import { useContext, useEffect, useState } from "react";
import { UserContext } from "../App";
import { Link } from "react-router-dom";

export default function GenreList() {
  const [data, setData] = useState({ genres: [], state: "loading" });
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function func() {
      const response = await userContext.authFetch("api/genres", {});

      if (response.ok) {
        setData({ genres: await response.json(), state: "ok" });
      } else {
        setData({ genres: [], state: "fail" });
      }
    }

    func();
  }, []);

  return (
    <div>
      <h1>Genres</h1>
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
            {data.genres.map((g) => (
              <tr key={g.genreId}>
                <td><Link to={`/genres/${g.genreId}`}>{g.name}</Link></td>
                <td>{g.books.length}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
