import { useRouteError, isRouteErrorResponse } from "react-router-dom";
import { ApplicationError } from "../../common/ApplicationError";
import NotFound from "./NotFound";
import styles from "./RouteErrorBoundary.module.css";

export default function RouteErrorBoundary() {
  const error = useRouteError();

  if (isRouteErrorResponse(error)) {
    return (
      <div className={styles.container}>
        <h1 className={styles.title}>
          {error.status} - {error.statusText}
        </h1>
      </div>
    );
  }

  if (error instanceof ApplicationError) {
    if (error.statusCode === 404) {
      return <NotFound />;
    }
    
    return (
      <div className={styles.container}>
        <h1 className={styles.title}>Fault Code: {error.statusCode}</h1>
        <p className={styles.message}>{error.message}</p>
      </div>
    );
  }

  const rawError = error as Error;
  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Catastrophic System Failure</h1>
      <pre className={styles.message}>{rawError.message || "Unknown execution failure"}</pre>
    </div>
  );
}
