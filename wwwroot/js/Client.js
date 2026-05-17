/* ── PANEL SWITCH ── */
function showPanel(id) { document.querySelectorAll('.panel').forEach(p => p.classList.remove('active')); const el = document.getElementById(id); if (el) el.classList.add('active'); }
function navTo(el, panelId) { document.querySelectorAll('.ni').forEach(n => n.classList.remove('active')); el.classList.add('active'); showPanel(panelId); const lbl = el.querySelector('.nav-lbl'); const h = document.getElementById('hdr-title'); if (h && lbl) h.textContent = lbl.textContent; }
/* ── NOTIF ── */
function toggleNotif(id) { const p = document.getElementById(id); if (p) p.classList.toggle('open'); }
document.addEventListener('click', e => { document.querySelectorAll('.ndrop.open').forEach(p => { if (!e.target.closest('.hdr-btn') && !e.target.closest('.ndrop')) p.classList.remove('open'); }); });
/* ── EXAM TIMER ── */
let examSeconds = 7200, examTimer = null;
function startExamTimer() { if (examTimer) return; examTimer = setInterval(() => { examSeconds--; updateTimerDisplay(); if (examSeconds <= 0) { clearInterval(examTimer); document.getElementById('timer-val').textContent = '00:00:00'; document.getElementById('timer-val').className = 'timer-val danger'; } }, 1000); }
function updateTimerDisplay() { const h = Math.floor(examSeconds / 3600), m = Math.floor((examSeconds % 3600) / 60), s = examSeconds % 60; const str = `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`; const el = document.getElementById('timer-val'); if (el) { el.textContent = str; el.className = 'timer-val' + (examSeconds < 600 ? ' danger' : examSeconds < 1800 ? ' warn' : ''); } }
/* ── SELECT OPTION ── */
function selectOpt(el) { el.closest('.q-options').querySelectorAll('.q-opt').forEach(o => o.classList.remove('selected')); el.classList.add('selected'); }
/* ── FACE VERIFY SIM ── */
let verifyState = 'idle';
function startVerify() { verifyState = 'scanning'; document.getElementById('verify-btn').textContent = 'Scanning…'; document.getElementById('verify-btn').disabled = true; document.getElementById('fv-s1').className = 'fv-step done'; document.getElementById('fv-s2').className = 'fv-step active'; setTimeout(() => { document.getElementById('fv-s2').className = 'fv-step done'; document.getElementById('fv-s3').className = 'fv-step active'; setTimeout(() => { document.getElementById('fv-s3').className = 'fv-step done'; document.getElementById('verify-overlay').style.display = 'flex'; }, 1500); }, 2000); }
/* ── Q NAV ── */
function goQuestion(n) { document.querySelectorAll('.qn-btn').forEach((b, i) => { if (b.classList.contains('current')) b.classList.remove('current'); }); const btns = document.querySelectorAll('.qn-btn'); if (btns[n - 1]) btns[n - 1].classList.add('current'); }
/* ── TABS ── */
function switchTab(groupId, tabId) { const g = document.getElementById(groupId); if (!g) return; g.querySelectorAll('.tab-p').forEach(p => p.classList.remove('active')); g.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active')); const tp = document.getElementById(tabId); const tb = g.querySelector(`[data-tab="${tabId}"]`); if (tp) tp.classList.add('active'); if (tb) tb.classList.add('active'); }
