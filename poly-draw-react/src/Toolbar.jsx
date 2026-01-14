import Button from "./Button.jsx";

function Toolbar({ onUndo, onRedo}) {
  return (
      <div className="toolbar">
          <h1 className="h1">Polydraw Demo</h1>
          <div className="btn-group">
          <Button onClick={onUndo}>
              <svg
                  width="10"
                  height="10"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={1.5}
                  stroke="currentColor"
                  className="size-6"
              >
                  <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M9 15 3 9m0 0 6-6M3 9h12a6 6 0 0 1 0 12h-3"
                  />
              </svg>
          </Button>
          <Button onClick={onRedo}>
              <svg width="10"
                   height="10" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5}
                   stroke="currentColor" className="size-6">
                  <path strokeLinecap="round" strokeLinejoin="round" d="m15 15 6-6m0 0-6-6m6 6H9a6 6 0 0 0 0 12h3"/>
              </svg>
          </Button>
          </div>

      </div>
  );
}

export default Toolbar;
