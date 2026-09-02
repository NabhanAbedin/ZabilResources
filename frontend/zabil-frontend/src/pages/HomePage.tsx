import { useState } from "react";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import Hero from "../components/home/Hero";
import AboutSection from "../components/home/AboutSection";
import { clearToken, getIsAdmin, getToken } from "../lib/authToken";

const HomePage = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(() => Boolean(getToken()));
  const [isAdmin, setIsAdmin] = useState(() => getIsAdmin());

  const onSignOut = () => {
    clearToken();
    setIsLoggedIn(false);
    setIsAdmin(false);
  };

  return (
    <div className="flex min-h-screen flex-col">
      <Header isLoggedIn={isLoggedIn} isAdmin={isAdmin} onSignOut={onSignOut} />
      <main className="flex-1">
        <Hero />
        <AboutSection />
      </main>
      <Footer />
    </div>
  );
};

export default HomePage;
