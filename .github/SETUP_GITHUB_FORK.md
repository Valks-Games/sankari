### Setup GitHub Fork
1. Fork this repo
2. Install [Git scm](https://git-scm.com/downloads)
3. Clone your fork with `git clone https://github.com/<USERNAME>/Sankari` (replace `<USERNAME>` with your GitHub username)
4. Push and pull changes from your fork with `git pull` `git push`
5. Create a pull request through the GitHub website to merge your work with this repo

### How to Delete Commits From Your Fork
```bash
# Delete all commits except for <last_working_commit_id>
git reset --hard <last_working_commit_id>

# Push the changes (be sure that this is what you really want to do or you may lose a lot of progress)
git push --force
```

### How to Fetch the Latest Updates From This Repo to Your Fork
```bash
# Add upstream as a remote (check remotes with git remote -v)
git remote add upstream https://github.com/Valks-Games/sankari.git

# Fetch data from upstream
git fetch upstream

# Merge upstream with your fork (if you don't care about your history, then replace merge with rebase)
git merge upstream/main
```
