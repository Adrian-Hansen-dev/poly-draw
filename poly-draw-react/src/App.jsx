import "./App.css";
import PolyCanvas from "./PolyCanvas.jsx";
import Toolbar from "./Toolbar.jsx";
import { useState } from "react";

function App() {
  const [points, setPoints] = useState([]);
  const [polygons, setPolygons] = useState([]);

  const revertLastDraw = () => {
    setPoints(points.slice(0, -1));
    if (points.length === 0 && polygons.length > 0) {
      const newPolygons = [...polygons];
      const lastPolygon = newPolygons.pop();
      setPolygons(newPolygons);
      setPoints(lastPolygon);
    }
  };
  return (
    <div className="app">
      <Toolbar onClick={revertLastDraw}></Toolbar>
      <PolyCanvas
        points={points}
        setPoints={setPoints}
        polygons={polygons}
        setPolygons={setPolygons}
      />
    </div>
  );
}

export default App;
