namespace OwnPolygonDrawing

open Expecto

module Tests =
    let baseModel =
        { canvasSize = { width = 1000.0; height = 600.0 }
          currentPolyline = None
          finishedPolygons = []
          mousePos = None
          past = None
          future = None }

    // Ignore history and mouse cursor for state comparison.
    let stripHistory (m : Model) =
        { m with past = None; future = None; mousePos = None }

    // Helper to apply AddPoint in a single call.
    let addPoint x y model =
        OwnPolygonDrawing.update (AddPoint (x, y)) model |> fst

    let ownPolygonDrawingTests =
        testList "OwnPolygonDrawing" [
            test "addPointStartsPolyline" {
                // First point creates a new current polyline.
                let result = addPoint 10.0 20.0 baseModel
                Expect.equal result.currentPolyline (Some [{ x = 10.0; y = 20.0 }]) "current polyline starts"
            }

            test "addPointPrepends" {
                // Points are prepended to keep O(1) insertion.
                let result =
                    baseModel
                    |> addPoint 1.0 1.0
                    |> addPoint 2.0 2.0
                Expect.equal result.currentPolyline (Some [{ x = 2.0; y = 2.0 }; { x = 1.0; y = 1.0 }]) "points are prepended"
            }

            test "finishPolygonMovesCurrentToFinished" {
                // Finishing moves the active polyline to the finished list.
                let result =
                    baseModel
                    |> addPoint 1.0 1.0
                    |> OwnPolygonDrawing.update FinishPolygon
                    |> fst
                Expect.equal result.currentPolyline None "current polyline cleared"
                Expect.equal result.finishedPolygons.Length 1 "finished polygon added"
            }

            test "finishPolygonOnEmptyIsSafe" {
                // Finishing with no current polyline should be a no-op.
                let result = OwnPolygonDrawing.update FinishPolygon baseModel |> fst
                Expect.equal (stripHistory result) (stripHistory baseModel) "no changes when no current polyline"
            }

            test "undoRedoRestoresState" {
                // Undo restores the previous state and redo returns to the newer one.
                let m1 = addPoint 5.0 6.0 baseModel
                let mUndo = OwnPolygonDrawing.update Undo m1 |> fst
                Expect.equal (stripHistory mUndo) (stripHistory baseModel) "undo returns to base"
                let mRedo = OwnPolygonDrawing.update Redo mUndo |> fst
                Expect.equal (stripHistory mRedo) (stripHistory m1) "redo restores last state"
            }
        ]
