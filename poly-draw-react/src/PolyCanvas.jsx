import { Canvas } from "@react-three/fiber";
import { Line, OrthographicCamera } from "@react-three/drei";
import { useState } from "react";

function PolyCanvas({ points, setPoints, polygons, setPolygons }) {
  const [preview, setPreview] = useState([]);

  return (
    <div className="canvas">
      <Canvas>
        <mesh
          position={[0, 0, -1]}
          onPointerMove={(e) => {
            const { x, y } = e.point;
            if (points.length > 0) {
              setPreview([points[points.length - 1], [x, y]]);
            }
          }}
          onPointerOut={() => {
            setPreview([]);
          }}
          onClick={(e) => {
            const { x, y } = e.point;
            setPoints((prev) => [...prev, [x, y]]);
          }}
          onDoubleClick={(e) => {
            const { x, y } = e.point;
            setPoints((prev) => [...prev, [x, y]]);
            setPolygons((prev) => [...prev, points]);
            setPoints([]);
            setPreview([]);
          }}
        >
          <planeGeometry args={[100, 100]} />
          <meshBasicMaterial opacity={0} />
        </mesh>
        <OrthographicCamera makeDefault position={[0, 0, 10]} zoom={100} />
        {points.length > 1 && (
          <Line points={points} color="black" lineWidth={2} />
        )}
        {preview.length > 0 && (
          <Line points={preview} color="red" lineWidth={2} />
        )}
        <group>
          {polygons.map((poly, index) => (
            <Line key={index} points={poly} color="blue" lineWidth={2} closed />
          ))}
        </group>
      </Canvas>
    </div>
  );
}

export default PolyCanvas;
