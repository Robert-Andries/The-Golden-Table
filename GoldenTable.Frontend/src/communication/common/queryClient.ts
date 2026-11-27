import { QueryClient } from "@tanstack/react-query";
import { ApplicationError } from "../../common/ApplicationError";

const MAX_RETRIES = 3;

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // A 4xx (not found, forbidden, invalid request) will not change on retry, so fail fast
      retry: (failureCount, error) => {
        if (error instanceof ApplicationError && error.statusCode >= 400 && error.statusCode < 500) {
          return false;
        }
        return failureCount < MAX_RETRIES;
      },
    },
  },
});

export default queryClient;
