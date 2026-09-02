import { Route, Routes } from "react-router-dom";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import OAuthCallbackPage from "./pages/OAuthCallbackPage";
import CreatePostPage from "./pages/CreatePostPage";
import ManagementConsolePage from "./pages/ManagementConsolePage";
import RequireAdmin from "./components/auth/RequireAdmin";

function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/oauth/callback" element={<OAuthCallbackPage />} />
      <Route
        path="/admin"
        element={
          <RequireAdmin>
            <ManagementConsolePage />
          </RequireAdmin>
        }
      />
      <Route
        path="/admin/posts/new"
        element={
          <RequireAdmin>
            <CreatePostPage />
          </RequireAdmin>
        }
      />
    </Routes>
  );
}

export default App;
