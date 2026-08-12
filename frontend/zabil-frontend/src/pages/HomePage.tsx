import { useState } from "react";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import Hero from "../components/home/Hero";
import AboutSection from "../components/home/AboutSection";
import { clearToken, getToken } from "../lib/authToken";

const HomePage = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(() => Boolean(getToken()));

  const onSignOut = () => {
    clearToken();
    setIsLoggedIn(false);
  };

  return (
    <div className="flex min-h-screen flex-col">
      <Header isLoggedIn={isLoggedIn} onSignOut={onSignOut} />
      <main className="flex-1">
        <Hero />
        <AboutSection />
      </main>
      <Footer />
    </div>
  );
};

export default HomePage;
