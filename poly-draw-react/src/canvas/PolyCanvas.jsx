import { Canvas } from "@react-three/fiber";
import { Line, OrthographicCamera } from "@react-three/drei";
import { useState } from "react";

export function PolyCanvas() {
  const [points, setPoints] = useState([]);
  const [preview, setPreview] = useState([]);
  return (
    <div style={{ width: "100vw", height: "100vh" }}>
      <Canvas>
        <mesh
          position={[0, 0, -1]}
          onPointerMove={(e) => {
            const { x, y } = e.point;
            if (points.length > 0) {
              setPreview([points[points.length - 1], [x, y]]);
            }
          }}
          onClick={(e) => {
            const { x, y } = e.point;
            setPoints((prev) => [...prev, [x, y]]);
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
      </Canvas>
    </div>
  );
}
