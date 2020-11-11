class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  url "https://github.com/Azure/bicep/releases/download/v0.1.226-alpha/bicep-osx-x64"
  sha256 "199788e05c6da4ec5d219d92e634d437c3e431378ecc8b5f791c12fece1bea47"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
