const API = "https://itunes.apple.com/search";
const form = document.querySelector("#search-form");
const input = document.querySelector("#search-input");
const results = document.querySelector("#results");
const statusBox = document.querySelector("#status");
const title = document.querySelector("#results-title");
const count = document.querySelector("#result-count");
const player = document.querySelector("#player");
const audio = document.querySelector("#audio");
let activeButton = null;

function setStatus(message, show = true) {
  statusBox.textContent = message;
  statusBox.hidden = !show;
}

function artwork(url, size = 600) {
  return (url || "").replace(/\d+x\d+bb/, `${size}x${size}bb`);
}

function makeCard(track) {
  const card = document.createElement("article");
  card.className = "track-card";

  const artWrap = document.createElement("div");
  artWrap.className = "art-wrap";
  const img = document.createElement("img");
  img.src = artwork(track.artworkUrl100);
  img.alt = `جلد آلبوم ${track.collectionName || track.trackName}`;
  img.loading = "lazy";
  artWrap.append(img);

  const play = document.createElement("button");
  play.className = "play-overlay";
  play.type = "button";
  play.textContent = "▶";
  play.title = track.previewUrl ? "پخش پیش‌نمایش" : "پیش‌نمایش موجود نیست";
  play.disabled = !track.previewUrl;
  play.addEventListener("click", () => playTrack(track, play));
  artWrap.append(play);

  const meta = document.createElement("div");
  meta.className = "track-meta";
  const trackTitle = document.createElement("div");
  trackTitle.className = "track-title";
  trackTitle.textContent = track.trackName || "بدون عنوان";
  const artist = document.createElement("div");
  artist.className = "track-artist";
  artist.textContent = track.artistName || "هنرمند ناشناس";
  const bottom = document.createElement("div");
  bottom.className = "track-bottom";
  const album = document.createElement("span");
  album.className = "album-name";
  album.textContent = track.collectionName || "تک‌آهنگ";
  const link = document.createElement("a");
  link.className = "store-link";
  link.textContent = "صفحه آهنگ ↗";
  link.href = track.trackViewUrl || track.collectionViewUrl || "#";
  link.target = "_blank";
  link.rel = "noopener noreferrer";
  bottom.append(album, link);
  meta.append(trackTitle, artist, bottom);
  card.append(artWrap, meta);
  return card;
}

function playTrack(track, button) {
  if (!track.previewUrl) return;
  if (activeButton) activeButton.textContent = "▶";
  activeButton = button;
  document.querySelector("#player-art").src = artwork(track.artworkUrl100, 200);
  document.querySelector("#player-title").textContent = track.trackName || "بدون عنوان";
  document.querySelector("#player-artist").textContent = track.artistName || "";
  audio.src = track.previewUrl;
  player.classList.remove("hidden");
  button.textContent = "Ⅱ";
  audio.play().catch(() => {
    setStatus("پخش پیش‌نمایش ممکن نشد. اتصال اینترنت یا تنظیمات مرورگر را بررسی کن.", true);
  });
  audio.onended = () => {
    if (activeButton) activeButton.textContent = "▶";
    activeButton = null;
  };
}

async function searchMusic(term) {
  const query = term.trim();
  if (!query) {
    setStatus("اول نام یک آهنگ یا خواننده را وارد کن.");
    input.focus();
    return;
  }
  title.textContent = `نتایج جست‌وجو برای «${query}»`;
  count.textContent = "";
  results.replaceChildren();
  setStatus("در حال پیدا کردن آهنگ‌ها…");
  try {
    const url = new URL(API);
    url.search = new URLSearchParams({term: query, entity: "song", limit: "30", country: "US"}).toString();
    const response = await fetch(url);
    if (!response.ok) throw new Error("درخواست ناموفق بود");
    const data = await response.json();
    const tracks = (data.results || []).filter(item => item.kind === "song");
    if (!tracks.length) {
      setStatus("نتیجه‌ای پیدا نشد. عبارت دیگری را امتحان کن.");
      return;
    }
    setStatus("", false);
    count.textContent = `${tracks.length} آهنگ`;
    const fragment = document.createDocumentFragment();
    tracks.forEach(track => fragment.append(makeCard(track)));
    results.append(fragment);
  } catch (error) {
    console.error(error);
    setStatus("ارتباط با سرویس موسیقی برقرار نشد. اینترنت را بررسی کن و دوباره تلاش کن.");
  }
}

form.addEventListener("submit", event => {
  event.preventDefault();
  searchMusic(input.value);
});
document.querySelectorAll("[data-query]").forEach(button => {
  button.addEventListener("click", () => {
    input.value = button.dataset.query;
    searchMusic(button.dataset.query);
  });
});
document.querySelector("#close-player").addEventListener("click", () => {
  audio.pause();
  audio.removeAttribute("src");
  audio.load();
  if (activeButton) activeButton.textContent = "▶";
  activeButton = null;
  player.classList.add("hidden");
});
audio.addEventListener("pause", () => {
  if (activeButton && !audio.ended) activeButton.textContent = "▶";
});
audio.addEventListener("play", () => {
  if (activeButton) activeButton.textContent = "Ⅱ";
});

// نمایش چند آهنگ اولیه بدون نیاز به ورود یا کلید API
searchMusic("Imagine Dragons");
