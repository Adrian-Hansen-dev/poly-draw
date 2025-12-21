import "./App.css";
import PolyCanvas from "./PolyCanvas.jsx";
import Toolbar from "./Toolbar.jsx";

function App() {
  return (
    <div className="app" style={{}}>
      <Toolbar></Toolbar>
      <PolyCanvas />
    </div>
  );
}

export default App;
